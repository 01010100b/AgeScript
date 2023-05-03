using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class SetTargetPoint : Inlined
    {
        public override bool HasStringLiteral => false;

        public SetTargetPoint() : base()
        {
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-set-target-point {result.Memory.Intr0}");
        }
    }
}
