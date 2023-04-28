using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Comparisons
{
    internal class GreaterThanOrEquals : ComparisonIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileComparison(result, ">=", cl, result_address, ref_result_address);
        }
    }

    internal class GreaterThanOrEqualsPrecise : GreaterThanOrEquals
    {
        protected override Type ParameterType => Primitives.Precise;

        public GreaterThanOrEqualsPrecise() : base()
        {
            Name = "GreaterThanOrEquals";
        }
    }
}
