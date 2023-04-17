using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class MemoryCompiler
    {
        public static readonly string[] NAMED_GOALS = { "stack-ptr", "sp0", "sp1", "sp2", "sp3" };
        
        public void Compile(Script script, RuleList rules) 
        {
            var goal = 513;

            foreach (var name in NAMED_GOALS)
            {
                goal--;
                rules.Constants.Add(name, goal);
            }

            foreach (var variable in script.GlobalVariables.Values)
            {
                goal -= variable.Type.Size;
                variable.Address = goal;
            }

            goal -= script.Functions.Max(x => x.RegisterCount);
            script.RegisterBase = goal;

            foreach (var function in script.Functions)
            {
                var offset = 1;

                foreach (var variable in function.AllVariables)
                {
                    variable.Address = script.RegisterBase + offset;
                    offset += variable.Type.Size;
                }
            }

            goal -= script.Functions.Max(x => x.ReturnType.Size);
            script.CallResultBase = goal;

            rules.AddAction($"set-goal {script.RegisterBase} 20000");
        }
    }
}
