using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics.DUC
{
    internal class SetTargetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetTargetPoint() : base()
        {
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-set-target-point {result.Memory.Intr0}");
        }
    }
}
