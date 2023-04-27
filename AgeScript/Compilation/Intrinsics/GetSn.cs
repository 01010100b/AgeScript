using AgeScript.Compilation;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetSn : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetSn() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "sn", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("sn must be const expression.");
            }

            rules.AddAction($"up-modify-goal {script.Intr0} s:= {ce.Int}");
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
