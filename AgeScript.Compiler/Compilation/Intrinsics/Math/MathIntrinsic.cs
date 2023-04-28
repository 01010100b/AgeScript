using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Compiler.Language.Expressions;
using AgeScript.Compiler.Language;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace AgeScript.Compiler.Compilation.Intrinsics.Math
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

        protected void CompileMath2(CompilationResult result, string op, CallExpression cl,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} g:{op} {result.Memory.Intr1}");
            Utils.MemCopy2(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
