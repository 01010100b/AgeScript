using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Comparisons
{
    internal abstract class ComparisonIntrinsic : Intrinsic
    {
        public override bool HasStringLiteral => false;
        protected abstract Type ParameterType { get; }

        public ComparisonIntrinsic() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = ParameterType });
            Parameters.Add(new() { Name = "b", Type = ParameterType });
        }

        protected void CompileComparison(CompilationResult result, string op, CallExpression cl,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} c:= 0");
            result.Rules.StartNewRule($"up-compare-goal {result.Memory.Intr0} g:{op} {result.Memory.Intr1}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} c:= 1");
            result.Rules.StartNewRule();
            Utils.MemCopy(result, result.Memory.Intr2, result_address.Value, 1, false, ref_result_address);
        }
    }
}
