using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class CanBuildLine : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CanBuildLine() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "building_id", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[2], script.Intr3);
            rules.AddAction($"set-goal {script.Intr4} 0");

            rules.StartNewRule($"up-can-build-line {script.Intr0} {script.Intr1} g: {script.Intr3}");
            rules.AddAction($"set-goal {script.Intr4} 1");

            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
