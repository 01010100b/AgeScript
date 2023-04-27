using AgeScript.Compilation;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.DUC
{
    internal class SetTargetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetTargetPoint() : base()
        {
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-set-target-point {script.Intr0}");
        }
    }
}
