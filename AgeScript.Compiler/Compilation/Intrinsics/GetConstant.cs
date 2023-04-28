using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetConstant : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public GetConstant() : base()
        {
            ReturnType = Primitives.Int;
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            result.Rules.AddAction($"set-goal {result.Memory.Intr0} {cl.Literal.Replace("\"", string.Empty)}");
            Utils.MemCopy2(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
