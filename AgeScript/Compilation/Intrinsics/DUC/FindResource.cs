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
    internal class FindResource : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FindResource() : base()
        {
            ReturnType = Primitives.Int4;
            Parameters.Add(new() { Name = "resource", Type = Primitives.Int });
            Parameters.Add(new() { Name = "count", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"up-find-resource g: {script.Intr0} g: {script.Intr1}");

            if (result_address is not null)
            {
                rules.AddAction($"up-get-search-state {script.Intr0}");
                Utils.MemCopy(script, rules, script.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
            }
        }
    }
}
