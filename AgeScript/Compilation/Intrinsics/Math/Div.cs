using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Math
{
    internal class Div : MathIntrinsic
    {
        protected override Language.Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileMath(script, function, rules, "z/", cl, result_address, ref_result_address);
        }
    }

    internal class DivPrecise : Div
    {
        protected override Language.Type ParameterType => Primitives.Precise;

        public DivPrecise() : base()
        {
            Name = "Div";
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            base.CompileCall(script, function, rules, cl, script.SpecialGoal);

            rules.AddAction($"up-modify-goal {script.SpecialGoal} c:* 100");
            Utils.MemCopy(script, rules, script.SpecialGoal, result_address.Value, 1, false, ref_result_address);
        }
    }
}
