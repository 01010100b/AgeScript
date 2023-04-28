using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Linker
{
    public class Linker
    {
        public string Link(string per, IReadOnlyDictionary<string, int> jump_targets)
        {
            foreach (var jump in jump_targets)
            {
                per = per.Replace(jump.Key, jump.Value.ToString());
            }

            return per;
        }
    }
}
