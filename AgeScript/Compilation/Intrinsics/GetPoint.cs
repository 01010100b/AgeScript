using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetPoint : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPoint() : base()
        {
            ReturnType = Primitives.Int2;
            Parameters.Add(new() { Name = "position_type", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("position_type must be const expression.");
            }

            rules.AddAction($"up-get-point {ce.Int} {script.Intr0}");
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
