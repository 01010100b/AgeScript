using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal abstract class Inlined : Intrinsic
    {
        internal abstract void CompileCall(CompilationResult result, CallExpression cl,
            int? result_address = null, bool ref_result_address = false);
    }
}
