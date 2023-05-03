using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class FilterStatus : Inlined
    {
        public override bool HasStringLiteral => false;

        public FilterStatus() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "object_status", Type = Primitives.Int });
            Parameters.Add(new() { Name = "object_list", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-filter-status g: {result.Memory.Intr0} g: {result.Memory.Intr1}");
        }
    }
}
