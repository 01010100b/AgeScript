using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetPointDistance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPointDistance() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "point1", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "point2", Type = Primitives.Int2 });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr2);
            result.Rules.AddAction($"up-get-point-distance {result.Memory.Intr0} {result.Memory.Intr2} {result.Memory.Intr4}");
            Utils.MemCopy2(result, result.Memory.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }

    internal class GetPointDistancePrecise : GetPointDistance
    {
        public GetPointDistancePrecise() : base()
        {
            Name = "GetPointDistance";
            ReturnType = Primitives.Precise;
            Parameters.Clear();
            Parameters.Add(new() { Name = "point1", Type = Primitives.Precise2 });
            Parameters.Add(new() { Name = "point2", Type = Primitives.Precise2 });
        }
    }
}
