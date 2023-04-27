using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetRandom : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetRandom() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "max", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            var big_max = (int.MaxValue - 100000000) / 3;

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-get-precise-time 0 {script.Intr1}");
            rules.AddAction($"up-modify-goal {script.Intr1} g:mod {script.Intr0}");

            for (int i = 0; i < 3; i++)
            {
                rules.AddAction($"generate-random-number {big_max}");
                rules.AddAction($"up-get-fact 33 0 {script.Intr2}");
                rules.AddAction($"up-modify-goal {script.Intr1} g:+ {script.Intr2}");
            }

            rules.AddAction($"up-modify-goal {script.Intr1} g:mod {script.Intr0}");
            Utils.MemCopy(script, rules, script.Intr1, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}