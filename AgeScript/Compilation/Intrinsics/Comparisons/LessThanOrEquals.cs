using AgeScript.Compilation;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Comparisons
{
    internal class LessThanOrEquals : ComparisonIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileComparison(script, function, rules, "<=", cl, result_address, ref_result_address);
        }
    }

    internal class LessThanOrEqualsPrecise : LessThanOrEquals
    {
        protected override Type ParameterType => Primitives.Precise;

        public LessThanOrEqualsPrecise() : base()
        {
            Name = "LessThanOrEquals";
        }
    }
}
