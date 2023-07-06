using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class CreateGroup : Inlined
    {
        public override bool HasStringLiteral => false;

        public CreateGroup() : base()
        {
            Parameters.Add(new() { Name = "index", Type = Primitives.Int });
            Parameters.Add(new() { Name = "count", Type = Primitives.Int });
            Parameters.Add(new() { Name = "group", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            ExpressionCompiler.Compile(result, cl.Arguments[2], result.Memory.Intr2);
            result.Rules.AddAction($"up-create-group {result.Memory.Intr0} {result.Memory.Intr1} g: {result.Memory.Intr2}");
        }
    }
}
