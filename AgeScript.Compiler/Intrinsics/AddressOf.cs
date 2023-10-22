using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal class AddressOf : Inlined
    {
        public override bool HasStringLiteral => throw new NotImplementedException();

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            throw new NotImplementedException();
        }
    }
}
