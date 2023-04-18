using Compiler.Language;
using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation.Intrinsics
{
    internal class Equals : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Equals(Script script) : base(script)
        {
            Name = "Equals";
            ReturnType = script.Types["Bool"];
            Parameters.Add(new() { Name = "a", Type = script.Types["Int"] });
            Parameters.Add(new() { Name= "b", Type = script.Types["Int"] });
        }

        public override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? address)
        {
            if (address is null)
            {
                return;
            }

            ExpressionCompiler.CompileExpression(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.CompileExpression(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"set-goal {address} 0");
            rules.StartNewRule($"up-compare-goal {script.Intr0} g:== {script.Intr1}");
            rules.AddAction($"set-goal {address} 1");
            rules.StartNewRule();
        }
    }
}
