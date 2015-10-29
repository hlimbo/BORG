/*Author: Harvey Limbo
   Fall 2014
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Insertions into the hash table will correspond to declarations of variables and values in a program
 * NOTE: ONLY SUPPORTS INTEGERS, not floats or decimals.
 * 
 * CASCADING IS NOT SUPPORTED YET i.e. PRINT 2 * 3 + 7 will not work
 * note: intpreter may read empty character as string. Be really careful when writing code in notepad.
 */

//will be the hashtable used for BORG class
namespace Assignment4
{
    class HashTable
    {
        //hashtable variables
        private LinkedList<Variable>[] chainlist;
        private int size;
        //flag - variable does not exist.
        private const int flag = -999999;
        //unassigned - variable has no assigned value
        private const int unassigned = -91919;

        public HashTable()
        {
            size = 127;
            chainlist = new LinkedList<Variable>[size];
            initTable();
        }

        //HASH FUNCTIONS

        //the variable name and its associated value is stored together in the same location
        //thanks to the Variable class.
        //for the add function.
        public int hash(Variable variable)
        {
            //hold the calculations necessary to find the hash key of the value.
            int t = 0;

            //getting the ascii values of each character in the string in bytes
            string value = variable.varName;
            byte[] asciibytes = ASCIIEncoding.ASCII.GetBytes(value.ToString());

            //using the formula given in the assignment
            //e.g. ABC = ((65 * 1) + (66 * 2) + (67 * 3))%TABLESIZE
            for (int i = 0; i < asciibytes.Length;i++ )
                t += (int)asciibytes[i] * (i + 1);
            
            t = t % size;
           // Console.WriteLine("Variable is stored at location {0}", t);
            return t;
        }

        //going to need to overload the hash function for search
        public int hash(string varName)
        {
            //hold the calculations necessary to find the hash key of the value.
            int t = 0;
            //getting the ascii values of each character in the string in bytes
            byte[] asciibytes = ASCIIEncoding.ASCII.GetBytes(varName);

            //for debugging purposes.
           /* for (int i = 0; i < asciibytes.Length; i++)
            {
                Console.WriteLine(varName[i] + " - " + asciibytes[i]);
            }*/

            //using the formula given in the assignment
            //e.g. ABC = ((65 * 1) + (66 * 2) + (67 * 3))%TABLESIZE
            for (int i = 0; i < asciibytes.Length; i++)
                t += (int)asciibytes[i] * (i + 1);

            t = t % size;
           // Console.WriteLine("Variable is stored at location {0}", t);
            return t;
        }

        //default value of int value is unassigned if varName is only declared and not assigned
        public void add(string varName,int value = unassigned)
        {
            Variable variable = new Variable(varName, value);
            int index = hash(variable);
            LinkedList<Variable> root = chainlist[index];
            LinkedListNode<Variable> current = root.First;

            //if the chainlist's current index has no value stored inside.
            if (current == null)
            {
                root.AddFirst(variable);
            }
            else
            {
                chainlist[index].AddLast(variable);
            }

        }

        public int search(string varName)
        {
            int index = hash(varName);
            LinkedList<Variable> root = chainlist[index];
            LinkedListNode<Variable> current = root.First;

            //using the chaining collision resolution technique
            while (current != null)
            {
                //if the variable we are searching for is found
                //return the variable's value.
                if (varName.Equals(current.Value.varName))
                {
                    //return this if variable is unassigned
                    if (current.Value.value == unassigned)
                        return unassigned;
                    else
                        return current.Value.value;
                }

                current = current.Next;               
            }

            //return this value if the variable was not found.
            return flag;
        }

        //searches for the varName in the hashtable and modifies its value
        public void searchAndModify(string varName,int newValue)
        {
            int index = hash(varName);
            LinkedList<Variable> root = chainlist[index];
            LinkedListNode<Variable> current = root.First;

            //using the chaining collision resolution technique
            while (current != null)
            {
                //if the variable we are searching for is found
                //set the variable's value = to the assigned value
                if (varName.Equals(current.Value.varName))
                {
                    current.Value.setValue(newValue);
                    return;
                }

                current = current.Next;
            }
        }


        //this function clears and initializes a new LinkedList<Variable>();
        public void initTable()
        {
            for (int i = 0; i < size; i++)
                chainlist[i] = new LinkedList<Variable>();
        }

        //return the specified chain in the list
        public LinkedList<Variable> getChainList(int index)
        {
            return chainlist[index];
        }
    }
}


/*Author: Harvey Limbo
   Fall 2014
 */
