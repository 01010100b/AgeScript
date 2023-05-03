using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Intrinsics
{
    internal class BuildLine : Inlined
    {
        public override bool HasStringLiteral => false;

        public BuildLine() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "building_id", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr2);
            result.Rules.StartNewRule($"up-can-build-line 0 {result.Memory.Intr0} g: {result.Memory.Intr2}");
            result.Rules.AddAction($"up-build-line {result.Memory.Intr0} {result.Memory.Intr0} g: {result.Memory.Intr2}");
            result.Rules.StartNewRule();
        }
    }
}
