using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class GetObjectData : Inlined
    {
        public override bool HasStringLiteral => false;

        public GetObjectData() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "object_data", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("object_data must be const expression.");
            }

            result.Rules.AddAction($"up-get-object-data {ce.Int} {result.Memory.Intr0}");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
