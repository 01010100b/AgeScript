using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
{
    internal class Max : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath(result, "max", cl, result_address, ref_result_address);
        }
    }

    internal class MaxPrecise : Max
    {
        protected override Type ParameterType => Primitives.Precise;

        public MaxPrecise() : base()
        {
            Name = "Max";
        }
    }
}
