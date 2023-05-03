using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class TargetObjects : Inlined
    {
        public override bool HasStringLiteral => false;

        public TargetObjects() : base()
        {
            Parameters.Add(new() { Name = "option", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "action", Type = Primitives.Int });
            Parameters.Add(new() { Name = "formation", Type = Primitives.Int });
            Parameters.Add(new() { Name = "stance", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression c0)
            {
                throw new Exception("option must be const.");
            }

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

            result.Rules.AddAction($"up-target-objects {(c0.Bool ? 1 : 0)} {c1.Int} {c2.Int} {c3.Int}");
        }
    }
}
