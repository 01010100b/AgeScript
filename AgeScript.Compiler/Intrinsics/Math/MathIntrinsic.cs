using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Math
{
    internal abstract class MathIntrinsic : Intrinsic
    {
        public override bool HasStringLiteral => false;
        protected abstract Type ParameterType { get; }

        public MathIntrinsic() : base()
        {
            ReturnType = ParameterType;
            Parameters.Add(new() { Name = "a", Type = ParameterType });
            Parameters.Add(new() { Name = "b", Type = ParameterType });
        }

        protected void CompileMath(CompilationResult result, string op, CallExpression cl,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} g:{op} {result.Memory.Intr1}");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
