using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Math
{
    internal class Mul : MathIntrinsic
    {
        protected override Type ParameterType => Primitives.Int;

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            CompileMath(result, "*", cl, result_address, ref_result_address);
        }
    }

    internal class MulPrecise : Mul
    {
        protected override Type ParameterType => Primitives.Precise;

        public MulPrecise() : base()
        {
            Name = "Mul";
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            base.CompileCall(result, cl, result.Memory.ConditionGoal);

            result.Rules.AddAction($"up-modify-goal {result.Memory.ConditionGoal} c:z/ 100");
            Utils.MemCopy(result, result.Memory.ConditionGoal, result_address.Value, 1, false, ref_result_address);
        }
    }
}
