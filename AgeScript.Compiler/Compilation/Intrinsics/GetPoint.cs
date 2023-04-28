using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPoint() : base()
        {
            ReturnType = Primitives.Int2;
            Parameters.Add(new() { Name = "position_type", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("position_type must be const expression.");
            }

            result.Rules.AddAction($"up-get-point {ce.Int} {result.Memory.Intr0}");
            Utils.MemCopy2(result, result.Memory.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
