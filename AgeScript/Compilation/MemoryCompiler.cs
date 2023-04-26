using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    internal class MemoryCompiler
    {
        public const int JUMP_END = 20000;

        private const int STACK_START = 1;

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

            script.Error = goal;
            script.SpecialGoal = --goal;
            script.StackPtr = --goal;

            if (!ScriptCompiler.Settings.InlineMemCopy)
            {
                script.NonInlinedMemCopyReturnAddr = --goal;
            }

            // table lookup result below that as it doesn't get used with up functions

            goal -= TableCompiler.Modulus;
            script.TableResultBase = goal;

            // special registers

            script.Sp3 = --goal;
            script.Sp2 = --goal;
            script.Sp1 = --goal;
            script.Sp0 = --goal;
            script.Intr4 = --goal;
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

            // and this is the stack limit

            script.StackLimit = goal;

            if (script.StackLimit < 41)
            {
                throw new Exception("Not enough memory.");
            }
        }

        private void InitializeMemory(Script script, RuleList rules)
        {
            // initialize once

            
            rules.AddAction($"set-goal {script.SpecialGoal} 0");
            rules.StartNewRule();
            rules.AddAction($"set-goal {script.SpecialGoal} 1");
            rules.AddAction("disable-self");

            rules.StartNewRule($"goal {script.SpecialGoal} 0");
            var jmpid = Script.GetUniqueId();
            rules.AddAction($"up-jump-direct c: {jmpid}");
            rules.StartNewRule();

            rules.AddAction($"set-goal {script.Error} 0");

            foreach (var global in script.GlobalVariables.Values)
            {
                Utils.Clear(rules, global.Address, global.Type.Size);
            }

            rules.StartNewRule();
            var id = rules.CurrentRuleIndex;
            rules.ReplaceStrings(jmpid, id.ToString());

            // initialize each tick

            rules.AddAction($"set-goal {script.SpecialGoal} 0");
            rules.AddAction($"set-goal {script.StackPtr} {STACK_START}");
            Utils.Clear(rules, script.RegisterBase, script.RegisterCount);
            rules.AddAction($"set-goal {script.RegisterBase} {JUMP_END}");

            // don't run if error

            rules.StartNewRule($"up-compare-goal {script.Error} c:> 0");
            rules.AddAction($"up-chat-data-to-self \"Error: %d\" g: {script.Error}");
            rules.AddAction($"up-jump-direct c: {JUMP_END}");
            rules.StartNewRule();
        }
    }
}
