using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class GetGroupSize : Inlined
    {
        public override bool HasStringLiteral => false;

        public GetGroupSize() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "group", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-get-group-size g: {result.Memory.Intr0} {result.Memory.Intr1}");
            Utils.MemCopy(result, result.Memory.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
