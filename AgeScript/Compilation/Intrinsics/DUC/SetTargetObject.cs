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
    internal class SetTargetObject : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetTargetObject() : base()
        {
            Parameters.Add(new() { Name = "search_source", Type = Primitives.Int });
            Parameters.Add(new() { Name = "index", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("search_source must be const expression.");
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr0);
            rules.AddAction($"up-set-target-object {ce.Int} g: {script.Intr0}");
        }
    }
}
