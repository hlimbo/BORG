/*Author: Harvey Limbo*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Nested block statements do not work as intended.
//NOTE - CHANGE PATHING in BORG file so the file gets properly read.
namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            BORG test = new BORG();
            //test file given via BlackBoard
            test.readFile("TheUltimateTest.txt");
            //my file with the extra credit function WRITE being used.
            //uncomment this line to see it work.
            //test.readFile("test.txt");
            Console.ReadLine();
        }
    }
}






















/*Author: Harvey Limbo
   Fall 2014
 */
