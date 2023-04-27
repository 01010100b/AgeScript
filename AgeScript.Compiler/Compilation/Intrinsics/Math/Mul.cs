using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
{
    internal class Mul : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            CompileMath(script, function, rules, "*", cl, result_address, ref_result_address);
        }
    }

    internal class MulPrecise : Mul
    {
        protected override Type ParameterType => Primitives.Precise;

        public MulPrecise() : base()
        {
            Name = "Mul";
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            base.CompileCall(script, function, rules, cl, script.SpecialGoal);
            rules.AddAction($"up-modify-goal {script.SpecialGoal} c:z/ 100");
            Utils.MemCopy(script, rules, script.SpecialGoal, result_address.Value, 1, false, ref_result_address);
        }
    }
}
