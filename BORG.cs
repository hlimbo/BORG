using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    class BORG
    {
        private string[] keywords = { "COM", "WRITE", "START", "FINISH", "VAR", "PRINT", "=" };
        private string[] operators = { "+", "-", "*", "/", "%", "^", "++", "--" };
        //flag - variable does not exist.
        private const int flag = -999999;
        //unassigned - variable has no assigned value
        private const int unassigned = -91919;
        //may need to change this into a list for scope.
        private HashTable table;
        //experimental - each block of code will hold a hashtable of memory.
        private Stack<HashTable> tables;

        public BORG()
        {
            table = new HashTable();
            //experimental
            tables = new Stack<HashTable>();
        }


        public bool isKeyWord(string word)
        {
            foreach (string key in keywords)
            {
                if (word.Equals(key))
                    return true;
            }

            return false;
        }

        public bool isOperator(string token)
        {
            foreach (string key in operators)
            {
                if (token.Equals(key))
                    return true;
            }

            return false;
        }

        //keywords for this interpreter!!
        /*
         * COM
         * WRITE
         * START-FINISH
         * VAR
         * ++
         * --
         * SUPPORTED ARITHMETIC: +, -, *, /, %, ^
         * PRINT
         * 
         * in addition, the interpreter needs to handle errors.
         */

        //this function determines which keyword was used in the text file to interpret.
        public void interpret(string line, int linenumber)
        {
            //interprets one line at a time in the text file.
            char[] separator = { ' ', '\n' };
            string[] words = line.Split(separator);

            for (int i = 0; i < words.Length; i++)
            {
                if (isKeyWord(words[i]))
                {
                    switch (words[i])
                    {
                        case "START":
                            start(words, linenumber);
                            return;
                        case "FINISH":
                            return;
                        case "COM":
                            return;
                        case "WRITE":
                            write(words);
                            return;
                        case "VAR":
                            declareVariable(words, linenumber);
                            break;
                        case "PRINT":
                            print(words, linenumber);
                            break;
                        case "=":
                            assignVariable(words, linenumber, i);
                            break;
                        default:
                            Console.WriteLine("Undefined operation at line {0}", linenumber);
                            break;
                    }
                }
                else
                {
                    if (words[i].Equals("++") || words[i].Equals("--"))
                        handleExpression(words, linenumber);
                }

            }

        }

        // EXTRA CREDIT FUNCTION - reads input from text file and displays its text on console
        //e.g. WRITE hi my name is bob
        //output - hi my name is bob
        /*
         * it will also support escape sequence formatting such as \n for newline and \t for tab
         * e.g. WRITE john went to school today \n he was happy to see his friends
         * output - john went to school today
         *         he was happy to see his friends
         *   
         * Escape Sequences
         * \n - newline
         * \t - tab
         * \u - all uppercase
         * \l - all lowercase
         */
        public void write(string[] words)
        {
            bool isUpper = false;
            bool isLower = false;
            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Equals("\\n"))
                    Console.WriteLine();
                else if (words[i].Equals("\\t"))
                    Console.Write("     ");
                else if(words[i].Equals("\\u"))
                    isUpper = true;
                else if(words[i].Equals("\\l"))
                    isLower = true;
                else
                {
                    if(isLower)
                        words[i] = words[i].ToLower();
                    else if(isUpper)
                        words[i] = words[i].ToUpper();

                    Console.Write(words[i] + " ");
                }
            }

            Console.WriteLine();
        }

        //The Scope functions**************************************************
        public void start(string[] words, int linenumber)
        {
            if (words.Length > 1)
            {
                Console.WriteLine("Error: unnecessary gibber gabber (words) added onto the keyword START on line {0}", linenumber);
                return;
            }

        }

        public void finish(string[] words, int linenumber)
        {
        }
        //***************************************************************
        public void declareVariable(string[] words, int linenumber)
        {
            //variable cannot start with numerical digits or non-alphabetical characters.
            string bad = "1234567890!@#$%^&*()-_=+[]{}|';:/?.>,<\"\\`~";

            for (int i = 0; i < bad.Length; i++)
            {
                //first character in the variable word
                if (words[1][0].Equals(bad[i]))
                {
                    Console.WriteLine("Error: variable cannot start with numerical or non-alphabetical characters at line {0}.", linenumber);
                    return;
                }
            }

            foreach (string key in keywords)
            {
                if (words[1].Equals(key))
                {
                    Console.WriteLine("Error: cannot declare reserved words as a variable at line {0}.", linenumber);
                    return;
                }
            }

            //add the declaration to the hashtable with an unassigned value
            table.add(words[1]);


        }

        public void assignVariable(string[] words, int linenumber, int adjustor)
        {
            string varName = words[adjustor - 1];
            string value = words[adjustor + 1];
            int result = unassigned;

            if (words.Length > 4 || words.Length == 2)
            {
                //looking for the operator.
                for (int i = 0; i < words.Length; i++)
                {
                    if (isOperator(words[i]))
                    {
                        //handle the arithmetic here and not in the interpreter
                        result = handleExpression(words, linenumber);
                        break;
                    }
                }
            }
            else if (words.Length == 3 || words.Length == 4)
            {
                if (table.search(varName) == flag)
                {
                    Console.WriteLine("Error: undefined variable at line {0}", linenumber);
                    return;
                }
                if (!int.TryParse(value, out result))
                {
                    if (table.search(value) == flag || table.search(value) == unassigned)
                    {
                        Console.WriteLine("Error: undefined variable at line {0}", linenumber);
                        return;
                    }

                    result = table.search(value);
                }
                else
                    result = int.Parse(value);
            }
            // * USE COLLISION RESOLUTION CHAINING TECH

            int index = table.hash(varName);
            LinkedList<Variable> root = table.getChainList(index);
            LinkedListNode<Variable> current = root.First;

            while (current != null)
            {
                //if the variable we are searching for is found
                //set the variable's value = to the assigned value
                if (varName.Equals(current.Value.varName))
                {
                    current.Value.setValue(result);
                    return;
                }

                current = current.Next;
            }

        }

        //handles the following: 2 + 3, a + b, a * b, etc.
        public int handleExpression(string[] expression, int linenumber)
        {
            int result = unassigned;
            for (int i = 0; i < expression.Length; i++)
            {
                switch (expression[i])
                {
                    case "++":
                        result = increment(expression, linenumber, i);
                        break;
                    case "--":
                        result = decrement(expression, linenumber, i);
                        break;
                    case "+":
                        result = addition(expression, linenumber, i);
                        break;
                    case "-":
                        result = subtraction(expression, linenumber, i);
                        break;
                    case "*":
                        result = multiply(expression, linenumber, i);
                        break;
                    case "/":
                        result = divide(expression, linenumber, i);
                        break;
                    case "%":
                        result = modulo(expression, linenumber, i);
                        break;
                    case "^":
                        result = exponent(expression, linenumber, i);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        //if printing out an existing variable, we need to search for that value in the hashtable.
        //otherwise if the keyword print is used. it prints out whatever is after it.
        public void print(string[] words, int linenumber)
        {
            string varToSearch = words[1];
            int total = 0;
            int num;
            int value = table.search(varToSearch);

            //error checking to see if any of the variables in the print call were undeclared or unassigned a value
            for (int j = 1; j < words.Length; j++)
            {
                int value2 = table.search(words[j]);
                if (!int.TryParse(words[j], out num) && !isOperator(words[j]) && (value2 == flag || value2 == unassigned))
                {
                    Console.WriteLine("Error: variable is undefined at line {0}", linenumber);
                    return;
                }
            }

            if (words.Length == 2)
            {
                Console.WriteLine("{0} is {1}", varToSearch, value);
            }
            else
            {
                total = handleExpression(words, linenumber);

                if (total == flag)
                    Console.WriteLine("Error: variable undefined at line {0}", linenumber);
                else if (total == unassigned)
                    Console.WriteLine("Error: variable unassigned at line {0}", linenumber);
                else
                {
                    for (int i = 1; i < words.Length; i++)
                        Console.Write(words[i] + " ");
                    Console.Write("is {0}\n", total);
                }
            }


        }

        public int increment(string[] words, int linenumber, int adjustor)
        {
            string varName = words[adjustor - 1];

            int value = table.search(varName);
            value++;
            table.searchAndModify(varName, value);

            return value;

        }

        public int decrement(string[] words, int linenumber, int adjustor)
        {
            string varName = words[adjustor - 1];
            int value = table.search(varName);

            if (value == flag)
                return flag;
            else if (value == unassigned)
                return unassigned;
            else
            {
                value--;
                table.searchAndModify(varName, value);
            }

            return value;

        }

        //we need to return the sum
        //imagine the standard case - a = a + b
        //can only add 2 numbers at a time when the variable has already been declared.
        public int addition(string[] tokens, int linenumber, int adjustor)
        {
            //template for all operations
            int sum = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;


            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    sum += value;
            }
            else
                sum = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            sum = sum + n2;

            //so a new variable doesn't get initialized by accident
            //or an existing varible's value doesn't get modified by accident
            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, sum);

            return sum;
        }

        //var name = 2 - 1
        //can only subtract two numbers at a time when the variable is already declared
        public int subtraction(string[] tokens, int linenumber, int adjustor)
        {
            int diff = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;

            //checks to see if lhs and rhs are variables
            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    diff = value;
            }
            else
                diff = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            diff = diff - n2;

            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, diff);

            return diff;
        }

        public int multiply(string[] tokens, int linenumber, int adjustor)
        {
            int product = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;

            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    product = value;
            }
            else
                product = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            product = product * n2;

            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, product);

            return product;
        }

        public int divide(string[] tokens, int linenumber, int adjustor)
        {
            int quotient = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;

            //checks to see if lhs and rhs are variables
            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    quotient = value;
            }
            else
                quotient = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            quotient = quotient / n2;

            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, quotient);

            return quotient;
        }

        public int exponent(string[] tokens, int linenumber, int adjustor)
        {
            int result = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;

            //checks to see if lhs and rhs are variable
            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    result = value;
            }
            else
                result = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            int base1 = result;
            for (int i = 1; i < n2; i++)
            {
                result = result * base1;
            }

            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, result);

            return result;
        }

        public int modulo(string[] tokens, int linenumber, int adjustor)
        {
            int result = 0;
            int n2;
            string thevariable = tokens[adjustor - 2];
            string lhs = tokens[adjustor - 1];
            string rhs = tokens[adjustor + 1];
            int num;

            //checks to see if lhs and rhs are variables
            if (!int.TryParse(lhs, out num))
            {
                int value = table.search(lhs);
                if (value == flag)
                    return flag;
                else
                    result = value;
            }
            else
                result = int.Parse(lhs);

            if (!int.TryParse(rhs, out num))
            {
                int value = table.search(rhs);
                if (value == flag)
                    return flag;
                else
                    n2 = value;
            }
            else
                n2 = int.Parse(rhs);

            result = result % n2;

            if (adjustor - 3 >= 0 && table.search(thevariable) != flag)
                table.searchAndModify(thevariable, result);

            return result;
        }

        //file reader function - i need to implement the interpreter for this.
        //supports .txt file format only
        public void readFile(string filename)
        {
            //change the directory of the file here.
            string path = "C:\\Users\\Harvey\\Desktop\\" + filename;
            int linenumber = 1;
            bool writeOK = false;
            int startCount = 0;
            int finishCount = 0;

            try
            {

                StreamReader scopereader = new StreamReader(path);
                string scopeline = scopereader.ReadLine();
                int scopelinenumber = 1;
                int firstStart = -1;
                int lastFinish = -1;
        
                //Read the entire file to make sure there are an even amount of STARTs and FINISHs in the file.
                while (scopeline != null)
                {
                    if (scopeline.Equals("START"))
                    {
                        if (firstStart == -1)
                        {
                            firstStart = scopelinenumber;
                        }
                        startCount++;
                    }
                    if (scopeline.Equals("FINISH"))
                    {
                        lastFinish = scopelinenumber;
                        finishCount++;
                    }
                    scopeline = scopereader.ReadLine();
                    scopelinenumber++;
                }

                scopereader.Close();

                if (startCount != finishCount)
                {
                    Console.WriteLine("Error: Too many or too few STARTs or FINISHs");
                    return;
                }

                //only implementation needed for this assignment are nested block statements
                //reads the file and outputs any calculations made with PRINT.
                StreamReader reader = new StreamReader(path);
                string line = reader.ReadLine();
                char[] separator = {' ','\n'};
                string[] words = line.Split(separator);

                Console.WriteLine("initial startcount: " + startCount);
                Console.WriteLine("initial finishcount: " + finishCount);

                while (line != null)
                {
                    if (line.Equals("START"))
                    {
                        writeOK = true;
                        startCount--;
                        //experimental
                        tables.Push(new HashTable());
                        
                    }
                     else if (words[0].Equals("COM"))
                     {
                         interpret(line, linenumber);
                     }
                    else if (writeOK)
                    {
                        if (finishCount == 0 && startCount == 0)
                        {
                            writeOK = false;
                        }
                        else if (line.Equals("FINISH"))
                        {
                            finishCount--;

                            if (lastFinish == linenumber)
                            {
                                //erases everything in the OUTER START-FINISH block.
                                table.initTable();
                                break;
                            }
                        }
                        else
                        {
                            interpret(line, linenumber);
                        }
                    }
                    else
                    {
                        if (!line.Equals("FINISH"))
                            Console.WriteLine("Error: Writing without START at line {0}", linenumber);

                        break;
                    }

                    line = reader.ReadLine();
                    linenumber++;
                    words = line.Split(separator);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
