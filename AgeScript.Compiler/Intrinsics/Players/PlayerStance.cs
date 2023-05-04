using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.Players
{
    internal class PlayerStance : Inlined
    {
        public override bool HasStringLiteral => false;

        public PlayerStance() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "player", Type = Primitives.Int });
            Parameters.Add(new() { Name = "stance", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[1] is not ConstExpression c1)
            {
                throw new Exception("stance must be const.");
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-modify-sn 251 g:= {result.Memory.Intr0}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:= 0");

            result.Rules.StartNewRule($"players-stance focus-player {c1.Int}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr1} c:= 1");
            result.Rules.StartNewRule();
            Utils.MemCopy(result, result.Memory.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
