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
            SetAddresses(script);
            InitializeMemory(script, rules);
        }

        private void SetAddresses(Script script)
        {
            // the stack grows upwards starting from goal 1
            // only goals 41-508 are usable with all up functions
            // the stack never gets used for up functions, only for copying from/to registers
            // so let the stack grow into the unusable range 1-40

            var goal = ScriptCompiler.Settings.MaxGoal;

            // special goals at the end as they won't be used with up functions

            script.SpecialGoal = goal;
            script.StackPtr = --goal;

            // table lookup result below that as it doesn't get used with up functions

            goal -= ScriptCompiler.Settings.MaxElementsPerRule - 2;
            script.TableResultBase = goal;

            // special registers

            script.Sp3 = --goal;
            script.Sp2 = --goal;
            script.Sp1 = --goal;
            script.Sp0 = --goal;
            script.Intr3 = --goal;
            script.Intr2 = --goal;
            script.Intr1 = --goal;
            script.Intr0 = --goal;

            // globals below that

            foreach (var variable in script.GlobalVariables.Values)
            {
                goal -= variable.Type.Size;
                variable.Address = goal;
            }

            // registers below that

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

            // call result below that

            goal -= script.Functions.Max(x => x.ReturnType.Size);
            script.CallResultBase = goal;
        }

        private void InitializeMemory(Script script, RuleList rules)
        {
            // initialize memory each tick
            // TODO initialize globals once (disable-self or something)

            rules.AddAction($"set-goal {script.SpecialGoal} 0");
            rules.AddAction($"set-goal {script.StackPtr} 1");
            Utils.Clear(rules, script.RegisterBase, script.RegisterCount);
            rules.AddAction($"set-goal {script.RegisterBase} 20000");
        }
    }
}
