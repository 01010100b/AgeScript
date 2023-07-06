using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class ResetGroup : Inlined
    {
        public override bool HasStringLiteral => false;

        public ResetGroup() : base()
        {
            Parameters.Add(new() { Name = "group", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-reset-group g: {result.Memory.Intr0}");
        }
    }
}
