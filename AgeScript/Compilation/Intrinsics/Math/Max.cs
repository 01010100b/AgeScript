using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Math
{
    internal class Max : MathIntrinsic
    {
        protected override Language.Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileMath(script, function, rules, "max", cl, result_address, ref_result_address);
        }
    }

    internal class MaxPrecise : Max
    {
        protected override Language.Type ParameterType => Primitives.Precise;

        public MaxPrecise() : base()
        {
            Name = "Max";
        }
    }
}
