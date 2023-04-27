using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    internal class Memory
    {
        public const int JUMP_END = 20000;

        // goals
        public int NonInlinedMemCopyReturnAddr { get; private set; } // used when Settings.InlineMemCopy = false
        public int Error { get; private set; } // set when run-time error occurs
        public int SpecialGoal { get; private set; } // used for control-flow (if, ...) conditions, lookup return addresses, and such
        public int StackPtr { get; private set; } // points to next free stack goal
        public int Sp0 { get; private set; } // special purpose registers, for memcopy and such
        public int Sp1 { get; private set; }
        public int Sp2 { get; private set; }
        public int Sp3 { get; private set; }
        public int Intr0 { get; private set; } // special registers for intrinsics
        public int Intr1 { get; private set; }
        public int Intr2 { get; private set; }
        public int Intr3 { get; private set; }
        public int Intr4 { get; private set; }
        public int RegisterBase { get; private set; } // start of registers
        public int CallResultBase { get; private set; } // start of goals where result of a function call is stored
        public int TableResultBase { get; private set; } // start of lookup result
        public int DebugMaxStackSpaceUsed { get; private set; } // debug max stack space used

        // info
        public int RegisterCount { get; private set; } // number of registers
        public int StackLimit { get; private set; } // stack-ptr can not grow beyond this

        public IReadOnlyDictionary<string, int> JumpTargets { get; } = new Dictionary<string, int>();

        public Memory(Script script, RuleList rules, Settings settings)
        {
            SetAddresses(script, settings);
            InitializeMemory(rules, settings);
        }

        private void SetAddresses(Script script, Settings settings)
        {
            // the stack grows upwards starting from goal 1
            // only goals 41-508 are usable with all up functions
            // the stack never gets used for up functions, only for copying from/to registers
            // so let the stack grow into the unusable range 1-40

            var goal = settings.MaxGoal;

            // special goals at the end as they won't be used with up functions

            Error = goal;
            SpecialGoal = --goal;
            StackPtr = --goal;

            if (!settings.InlineMemCopy)
            {
                NonInlinedMemCopyReturnAddr = --goal;
            }

            if (settings.Debug)
            {
                DebugMaxStackSpaceUsed = --goal;
            }

            // table lookup result below that as it doesn't get used with up functions

            goal -= settings.TableModulus;
            TableResultBase = goal;

            // special registers

            Sp3 = --goal;
            Sp2 = --goal;
            Sp1 = --goal;
            Sp0 = --goal;
            Intr4 = --goal;
            Intr3 = --goal;
            Intr2 = --goal;
            Intr1 = --goal;
            Intr0 = --goal;

            // globals below that

            foreach (var variable in script.GlobalVariables.Values)
            {
                goal -= variable.Type.Size;
                variable.Address = goal;
            }

            // registers below that

            RegisterCount = script.Functions.Max(x => x.RegisterCount);
            goal -= RegisterCount;
            RegisterBase = goal;

            foreach (var function in script.Functions)
            {
                var offset = 1; // first register (offset 0) is return addr

                foreach (var variable in function.AllVariables)
                {
                    variable.Address = RegisterBase + offset;
                    offset += variable.Type.Size;
                }
            }

            // call result below that

            goal -= script.Functions.Max(x => x.ReturnType.Size);
            CallResultBase = goal;

            // and this is the stack limit

            StackLimit = goal;

            if (StackLimit < 41)
            {
                throw new Exception("Not enough memory.");
            }
        }

        private void InitializeMemory(RuleList rules, Settings settings)
        {
            // initialize everything once

            rules.AddAction($"set-goal {SpecialGoal} 0");
            rules.StartNewRule();
            rules.AddAction($"set-goal {SpecialGoal} 1");
            rules.AddAction("disable-self");
            
            rules.StartNewRule($"goal {SpecialGoal} 0");
            var jmpid = Script.GetUniqueId();
            rules.AddAction($"up-jump-direct c: {jmpid}");
            rules.StartNewRule();

            Utils.Clear(rules, 1, settings.MaxGoal);

            rules.StartNewRule();
            var id = rules.CurrentRuleIndex;
            rules.ReplaceStrings(jmpid, id.ToString());

            // initialize each tick

            rules.AddAction($"set-goal {SpecialGoal} 0");
            rules.AddAction($"set-goal {StackPtr} 1");
            Utils.Clear(rules, RegisterBase, RegisterCount);
            rules.AddAction($"set-goal {RegisterBase} {JUMP_END}");

            // don't run if error

            rules.StartNewRule($"up-compare-goal {Error} c:> 0");
            rules.AddAction($"up-chat-data-to-self \"ERROR: %d\" g: {Error}");
            rules.AddAction($"up-jump-direct c: {JUMP_END}");
            rules.StartNewRule();

            if (settings.Debug)
            {
                rules.AddAction($"set-goal {Intr0} {StackLimit}");
                rules.AddAction($"up-modify-goal {Intr0} g:- {DebugMaxStackSpaceUsed}");
                rules.AddAction($"up-chat-data-to-self \"stack remaining: %d\" g: {Intr0}");
            }
        }
    }
}
