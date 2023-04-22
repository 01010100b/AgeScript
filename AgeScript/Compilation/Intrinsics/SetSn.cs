using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class SetSn : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public SetSn()
        {
            Name = "SetSn";
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "sn", Type = Primitives.Int });
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (cl.Arguments[0] is not ConstExpression sn)
            {
                throw new Exception("sn must be const expression.");
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr0);
            rules.AddAction($"up-modify-sn {sn.Int} g:= {script.Intr0}");
        }
    }
}
