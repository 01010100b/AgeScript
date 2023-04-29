using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Boolean
{
    internal class And : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public And() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "b", Type = Primitives.Bool });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} c:= 0");
            result.Rules.StartNewRule($"and (goal {result.Memory.Intr0} 1) (goal {result.Memory.Intr1} 1)");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr2} c:= 1");
            result.Rules.StartNewRule();
            Utils.MemCopy(result, result.Memory.Intr2, result_address.Value, 1, false, ref_result_address);
        }
    }
}
