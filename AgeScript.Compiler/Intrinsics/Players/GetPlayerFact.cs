using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Intrinsics
{
    internal class GetPlayerFact : Inlined
    {
        public override bool HasStringLiteral => false;

        public GetPlayerFact() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "player", Type = Primitives.Int });
            Parameters.Add(new() { Name = "fact_id", Type = Primitives.Int });
            Parameters.Add(new() { Name = "fact_parameter", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[1] is not ConstExpression ce1 || cl.Arguments[2] is not ConstExpression ce2)
            {
                throw new Exception("fact_id and fact_parameter must be const expressions.");
            }

            if (cl.Arguments[0] is ConstExpression ce0 && ce0.Int < 0)
            {
                var player = "my-player-number";

                if (ce0.Int == -2)
                {
                    player = "target-player";
                }

                result.Rules.AddAction($"up-modify-sn sn-focus-player-number c:= {player}");
            }
            else
            {
                ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
                result.Rules.AddAction($"up-modify-sn sn-focus-player-number g:= {result.Memory.Intr0}");
            }

            result.Rules.AddAction($"up-get-focus-fact {ce1.Int} {ce2.Int} {result.Memory.Intr1}");
            Utils.MemCopy(result, result.Memory.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
