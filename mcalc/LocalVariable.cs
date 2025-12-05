using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcalc
{
    [Serializable]
    public class LocalVariable
    {
        public LocalVariable()
        {
        }
        public LocalVariable(string name, double value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
