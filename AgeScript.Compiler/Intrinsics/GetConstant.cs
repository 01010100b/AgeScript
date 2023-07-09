using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal class GetConstant : Inlined
    {
        public override bool HasStringLiteral => true;

        public GetConstant() : base()
        {
            ReturnType = Primitives.Int;
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} c:= {cl.Literal}");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
