using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class Equals : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Equals()
        {
            Name = "Equals";
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = Primitives.Int });
            Parameters.Add(new() { Name = "b", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules,
            CallExpression cl, int? address, bool ref_result_address)
        {
            if (address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"set-goal {address} 0");
            rules.StartNewRule($"up-compare-goal {script.Intr0} g:== {script.Intr1}");
            rules.AddAction($"set-goal {address} 1");
            rules.StartNewRule();
        }
    }
}
