using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    internal class Memory
    {
        // goals
        public int NonInlinedMemCopyReturnAddr { get; private set; } // used when Settings.InlineMemCopy = false
        public int Error { get; private set; } // set when run-time error occurs
        public int StackPtr { get; private set; } // points to next free stack goal
        public int ConditionGoal { get; private set; } // used for control-flow (if, ...) conditions, lookup return addresses
        public int AssignGoal { get; private set; } // for assign statements
        public int ExpressionGoal { get; private set; } // for expressions
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

        private Dictionary<Variable, int> VariableAddresses { get; } = new();

        public Memory(Script script, RuleList rules, Settings settings)
        {
            SetAddresses(script, settings);
            InitializeMemory(rules, settings);
        }

        public int GetAddress(Variable variable)
        {
            if (VariableAddresses.TryGetValue(variable, out var address))
            {
                return address;
            }
            else
            {
                throw new Exception("Variable not found.");
            }
        }

        public int GetRegisterCount(Function function) => function.AllVariables.Sum(x => x.Type.Size) + 1;

        private void SetAddresses(Script script, Settings settings)
        {
            // the stack grows upwards starting from goal 1
            // only goals 41-508 are usable with all up functions
            // the stack never gets used for up functions, only for copying from/to registers
            // so let the stack grow into the unusable range 1-40

            var goal = settings.MaxGoal;

            // special goals at the end as they won't be used with up functions

            Error = goal;
            StackPtr = --goal;
            ConditionGoal = --goal;
            AssignGoal = --goal;
            ExpressionGoal = --goal;

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
                VariableAddresses.Add(variable, goal);
            }

            // registers below that

            RegisterCount = script.Functions.Max(x => GetRegisterCount(x));
            goal -= RegisterCount;
            RegisterBase = goal;

            foreach (var function in script.Functions)
            {
                var offset = 1; // first register (offset 0) is return addr

                foreach (var variable in function.AllVariables)
                {
                    VariableAddresses.Add(variable, RegisterBase + offset);
                    offset += variable.Type.Size;
                }
            }

            // call result below that

            goal -= Math.Max(1, script.Functions.Max(x => x.ReturnType.Size));
            CallResultBase = goal;

            // and this is also the stack limit

            StackLimit = CallResultBase;

            if (StackLimit < 41)
            {
                throw new Exception("Not enough memory.");
            }
        }

        private void InitializeMemory(RuleList rules, Settings settings)
        {
            // initialize everything once

            rules.AddAction($"up-modify-goal {ConditionGoal} c:= 0");
            rules.StartNewRule();
            rules.AddAction($"up-modify-goal {ConditionGoal} c:= 1");
            rules.AddAction("disable-self");

            rules.StartNewRule($"goal {ConditionGoal} 0");
            var jump_target = rules.CreateJumpTarget();
            rules.AddAction($"up-jump-direct c: {jump_target}");
            rules.StartNewRule();

            Utils.Clear(rules, 1, settings.MaxGoal);

            rules.StartNewRule();
            rules.ResolveJumpTarget(jump_target);

            // initialize each tick

            rules.AddAction($"up-modify-goal {ConditionGoal} c:= 0");
            rules.AddAction($"up-modify-goal {StackPtr} c:= 1");
            Utils.Clear(rules, RegisterBase, RegisterCount);
            rules.AddAction($"up-modify-goal {RegisterBase} c:= {rules.EndTarget}");
        }
    }
}
