using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class CanTrain : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CanTrain() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "unit_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 0");
            result.Rules.StartNewRule($"up-can-train {result.Memory.Intr0} g: {result.Memory.Intr1}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 1");
            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr2, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
