using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.DUC
{
    internal class FilterStatus : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterStatus() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "object_status", Type = Primitives.Int });
            Parameters.Add(new() { Name = "object_list", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"up-filter-status g: {script.Intr0} g: {script.Intr1}");
        }
    }
}
