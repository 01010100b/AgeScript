using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Intrinsics
{
    internal class CanBuildLine : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CanBuildLine() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "building_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            ExpressionCompiler.Compile(result, cl.Arguments[2], result.Memory.Intr3);
            result.Rules.AddAction($"up-bound-precise-point {result.Memory.Intr1} 0 c: 0");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr4} c:= 0");

            result.Rules.StartNewRule($"up-can-build-line {result.Memory.Intr0} {result.Memory.Intr1} g: {result.Memory.Intr3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr4} c:= 1");

            result.Rules.StartNewRule();
            Utils.MemCopy(result, result.Memory.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
