using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Compiler.Language.Expressions;
using AgeScript.Compiler.Language;

namespace AgeScript.Compiler.Compilation.Intrinsics.Comparisons
{
    internal abstract class ComparisonIntrinsic : Intrinsic
    {
        public override bool HasStringLiteral => false;
        protected abstract Type ParameterType { get; }

        public ComparisonIntrinsic() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = ParameterType });
            Parameters.Add(new() { Name = "b", Type = ParameterType });
        }

        protected void CompileComparison(Script script, Function function, RuleList rules, string op,
            CallExpression cl, int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);

            if (ScriptCompiler.Settings.OptimizeComparisons && ref_result_address == false)
            {
                rules.AddAction($"set-goal {result_address} 0");
                rules.StartNewRule($"up-compare-goal {script.Intr0} g:{op} {script.Intr1}");
                rules.AddAction($"set-goal {result_address} 1");
            }
            else
            {
                rules.AddAction($"set-goal {script.Intr2} 0");
                rules.StartNewRule($"up-compare-goal {script.Intr0} g:{op} {script.Intr1}");
                rules.AddAction($"set-goal {script.Intr2} 1");
                rules.StartNewRule();
                Utils.MemCopy(script, rules, script.Intr2, result_address.Value, 1, false, ref_result_address);
            }
        }
    }
}
