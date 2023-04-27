using AgeScript.Compilation;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class Train : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Train() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "unit_id", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.StartNewRule($"up-can-train {script.Intr0} g: {script.Intr1}");
            rules.AddAction($"up-train {script.Intr0} g: {script.Intr1}");
            rules.StartNewRule();
        }
    }
}
