using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Comparisons
{
    internal class LessThan : ComparisonIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileComparison(script, function, rules, "<", cl, result_address, ref_result_address);
        }
    }

    internal class LessThanPrecise : LessThan
    {
        protected override Type ParameterType => Primitives.Precise;

        public LessThanPrecise() : base()
        {
            Name = "LessThan";
        }
    }
}
