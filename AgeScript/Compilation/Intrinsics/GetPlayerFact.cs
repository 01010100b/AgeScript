using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetPlayerFact : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPlayerFact() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "player", Type = Primitives.Int });
            Parameters.Add(new() { Name = "fact_id", Type = Primitives.Int });
            Parameters.Add(new() { Name = "fact_parameter", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[1] is not ConstExpression || cl.Arguments[2] is not ConstExpression)
            {
                throw new Exception("fact_id and fact_parameter must be const expressions.");
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-modify-sn sn-focus-player-number g:= {script.Intr0}");
            var fact_id = ((ConstExpression)cl.Arguments[1]).Int;
            var fact_parameter = ((ConstExpression)cl.Arguments[2]).Int;
            rules.AddAction($"up-get-focus-fact {fact_id} {fact_parameter} {script.Intr1}");
            Utils.MemCopy(script, rules, script.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
