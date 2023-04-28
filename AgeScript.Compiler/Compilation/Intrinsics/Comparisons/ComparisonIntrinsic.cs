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
                rules.StartNewRule();
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

        protected void CompileComparison2(CompilationResult result, string op, CallExpression cl,
            int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 0");
            result.Rules.StartNewRule($"up-compare-goal {result.Memory.Intr0} g:{op} {result.Memory.Intr1}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr2} 1");
            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr2, result_address.Value, 1, false, ref_result_address);
        }
    }
}
