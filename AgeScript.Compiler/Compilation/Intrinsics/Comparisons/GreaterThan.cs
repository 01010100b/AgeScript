using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Comparisons
{
    internal class GreaterThan : ComparisonIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileComparison(result, ">", cl, result_address, ref_result_address);
        }
    }

    internal class GreaterThanPrecise : GreaterThan
    {
        protected override Type ParameterType => Primitives.Precise;

        public GreaterThanPrecise() : base()
        {
            Name = "GreaterThan";
        }
    }
}
