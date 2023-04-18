using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class MemoryCompiler
    {
        public static readonly string[] NAMED_GOALS = { "sp0", "sp1", "sp2", "sp3" };
        
        public void Compile(Script script, RuleList rules) 
        {
            SetAddresses(script, rules);
            InitializeMemory(script, rules);
        }

        private void SetAddresses(Script script, RuleList rules)
        {
            var goal = Program.Settings.MaxGoal;
            script.StackPtr = goal;

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

            script.RegisterCount = script.Functions.Max(x => x.RegisterCount);
            goal -= script.RegisterCount;
            script.RegisterBase = goal;

            foreach (var function in script.Functions)
            {
                var offset = 1; // first register is return addr

                foreach (var variable in function.AllVariables)
                {
                    variable.Address = script.RegisterBase + offset;
                    offset += variable.Type.Size;
                }
            }

            goal -= script.Functions.Max(x => x.ReturnType.Size);
            script.CallResultBase = goal;
        }

        private void InitializeMemory(Script script, RuleList rules)
        {
            rules.AddAction($"set-goal {script.StackPtr} 1");

            for (int i = 0; i < script.RegisterCount; i++)
            {
                rules.AddAction($"set-goal {script.RegisterBase + i} 0");
            }

            rules.AddAction($"set-goal {script.RegisterBase} 20000");
        }
    }
}
