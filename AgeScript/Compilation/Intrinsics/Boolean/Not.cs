using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Boolean
{
    internal class Not : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Not()
        {
            Name = "Not";
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = Primitives.Bool });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"set-goal {script.Intr2} 0");
            rules.StartNewRule($"not (goal {script.Intr0} 1)");
            rules.AddAction($"set-goal {script.Intr2} 1");
            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr2, result_address.Value, 1, false, ref_result_address);
        }
    }
}
