using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Boolean
{
    internal class BitwiseNot : Inlined
    {
        public override bool HasStringLiteral => false;

        public BitwiseNot() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "a", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} c:= -1");
            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr1);
            result.Rules.AddAction($"up-modify-flag {result.Memory.Intr0} g:- {result.Memory.Intr1}");
            Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
