using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class FilterDistance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterDistance() : base()
        {
            Parameters.Add(new() { Name = "min", Type = Primitives.Int });
            Parameters.Add(new() { Name = "max", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-filter-distance g: {result.Memory.Intr0} g: {result.Memory.Intr1}");
        }
    }
}
