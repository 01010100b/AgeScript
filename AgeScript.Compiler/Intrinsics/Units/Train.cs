using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics
{
    internal class Train : Inlined
    {
        public override bool HasStringLiteral => false;

        public Train() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "unit_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.StartNewRule($"up-can-train {result.Memory.Intr0} g: {result.Memory.Intr1}");
            result.Rules.AddAction($"up-train {result.Memory.Intr0} g: {result.Memory.Intr1}");
            result.Rules.StartNewRule();
        }
    }
}
