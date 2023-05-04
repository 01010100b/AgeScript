using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Math
{
    internal class Lerp : Inlined
    {
        public override bool HasStringLiteral => false;

        public Lerp() : base()
        {
            ReturnType = Primitives.Int2;
            Parameters.Add(new() { Name = "point1", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "point2", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "amount", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr2);
            ExpressionCompiler.Compile(result, cl.Arguments[2], result.Memory.Intr4);
            result.Rules.AddAction($"up-lerp-tiles {result.Memory.Intr0} {result.Memory.Intr2} g: {result.Memory.Intr4}");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 2, false, ref_result_address);
        }
    }
}
