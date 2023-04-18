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
        public void Compile(Script script, RuleList rules) 
        {
            SetAddresses(script, rules);
            InitializeMemory(script, rules);
        }

        private void SetAddresses(Script script, RuleList rules)
        {
            // the stack grows upwards starting from goal 1
            // only goals 41-508 are usable with all up functions
            // the stack never gets used for up functions, only for copying from/to registers
            // so let the stack grow into the unusable range 1-40

            var goal = Program.Settings.MaxGoal;

            // special goals at the top as they won't be used with up functions

            script.CondGoal = goal;
            script.StackPtr = --goal;
            script.Sp3 = --goal;
            script.Sp2 = --goal;
            script.Sp1 = --goal;
            script.Sp0 = --goal;
            script.Intr3 = --goal; // first goal that could be used with up functions
            script.Intr2 = --goal;
            script.Intr1 = --goal;
            script.Intr0 = --goal;

            // globals below that as they are used with up functions

            foreach (var variable in script.GlobalVariables.Values)
            {
                goal -= variable.Type.Size;
                variable.Address = goal;
            }

            // registers below that as they are used with up functions

            script.RegisterCount = script.Functions.Max(x => x.RegisterCount);
            goal -= script.RegisterCount;
            script.RegisterBase = goal;

            foreach (var function in script.Functions)
            {
                var offset = 1; // first register (offset 0) is return addr

                foreach (var variable in function.AllVariables)
                {
                    variable.Address = script.RegisterBase + offset;
                    offset += variable.Type.Size;
                }
            }

            // call result below that as it used with up functions

            goal -= script.Functions.Max(x => x.ReturnType.Size);
            script.CallResultBase = goal;
        }

        private void InitializeMemory(Script script, RuleList rules)
        {
            rules.AddAction($"set-goal {script.CondGoal} 0");
            rules.AddAction($"set-goal {script.StackPtr} 1");
            Utils.Clear(rules, script.RegisterBase, script.RegisterCount);
            rules.AddAction($"set-goal {script.RegisterBase} 20000");
        }
    }
}
