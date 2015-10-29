/*Author: Harvey Limbo*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    class Variable
    {
        public int value { get; private set; }
        public string varName { get; private set; }

        public Variable(string name, int v)
        {
            varName = string.Copy(name);
            value = v;
        }

        public void setName(string name)
        {
            varName = string.Copy(name);
        }

        public void setValue(int v)
        {
            value = v;
        }

        //gets the variables name and value.
        public string get()
        {
            return varName + " = " + value;
        }

        //needed for comparison
        public bool Equal(Variable other)
        {
            return this.varName.Equals(other.varName) && this.value == other.value;
        }

        //may change || to &&
        public bool isEmpty()
        {
            return varName == null || value == 0;
        }
    }
}

















/*Author: Harvey Limbo
   Fall 2014
 */
