using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Linker
{
    public class ScriptLinker
    {
        public string Link(string jtp, IReadOnlyDictionary<string, int> jump_targets)
        {
            foreach (var jump in jump_targets)
            {
                jtp = jtp.Replace(jump.Key, jump.Value.ToString());
            }

            return jtp;
        }
    }
}
