using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class CanTrain : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CanTrain() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "escrow", Type =  Primitives.Bool });
            Parameters.Add(new() { Name = "unit_id", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"set-goal {script.Intr2} 0");
            rules.StartNewRule($"up-can-train {script.Intr0} g: {script.Intr1}");
            rules.AddAction($"set-goal {script.Intr2} 1");
            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr2, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
