using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
{
    internal class Div : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath2(result, "z/", cl, result_address, ref_result_address);
        }
    }

    internal class DivPrecise : Div
    {
        protected override Type ParameterType => Primitives.Precise;

        public DivPrecise() : base()
        {
            Name = "Div";
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            base.CompileCall2(result, cl, result.Memory.ConditionGoal);

            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:* 100");
            Utils.MemCopy2(result, result.Memory.ConditionGoal, result_address.Value, 1, false, ref_result_address);
        }
    }
}
