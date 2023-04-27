using AgeScript.Compilation;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Math
{
    internal class Mod : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileMath(script, function, rules, "mod", cl, result_address, ref_result_address);
        }
    }

    internal class ModPrecise : Mod
    {
        protected override Type ParameterType => Primitives.Precise;

        public ModPrecise() : base()
        {
            Name = "Mod";
        }
    }
}
