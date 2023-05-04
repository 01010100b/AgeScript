using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class TargetPoint : Inlined
    {
        public override bool HasStringLiteral => false;

        public TargetPoint() : base()
        {
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "action", Type = Primitives.Int });
            Parameters.Add(new() { Name = "formation", Type = Primitives.Int });
            Parameters.Add(new() { Name = "stance", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[1] is not ConstExpression c1)
            {
                throw new Exception("action must be const.");
            }

            if (cl.Arguments[2] is not ConstExpression c2)
            {
                throw new Exception("formation must be const.");
            }

            if (cl.Arguments[3] is not ConstExpression c3)
            {
                throw new Exception("stance must be const.");
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-sn 292 c:= 5");
            result.Rules.AddAction($"up-target-point {result.Memory.Intr0} {c1.Int} {c2.Int} {c3.Int}");
        }
    }
}
