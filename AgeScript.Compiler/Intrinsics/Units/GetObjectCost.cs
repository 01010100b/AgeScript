using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Units
{
    internal class GetObjectCost : Inlined
    {
        public override bool HasStringLiteral => false;

        public GetObjectCost() : base()
        {
            ReturnType = Primitives.Int4;
            Parameters.Add(new() { Name = "object_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr4);
            result.Rules.AddAction($"up-setup-cost-data 1 {result.Memory.Intr0}");
            result.Rules.AddAction($"up-add-object-cost g: {result.Memory.Intr4} c: 1");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 4, false, ref_result_address);
        }
    }
}