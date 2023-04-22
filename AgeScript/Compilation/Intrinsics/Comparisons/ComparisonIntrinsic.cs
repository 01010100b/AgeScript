﻿using AgeScript.Language.Expressions;
using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.Comparisons
{
    internal abstract class ComparisonIntrinsic : Intrinsic
    {
        public ComparisonIntrinsic() : base()
        {
            Name = GetType().Name;
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = Primitives.Int });
            Parameters.Add(new() { Name = "b", Type = Primitives.Int });
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
            rules.AddAction($"set-goal {script.Intr2} 0");
            rules.StartNewRule($"up-compare-goal {script.Intr0} g:{op} {script.Intr1}");
            rules.AddAction($"set-goal {script.Intr2} 1");
            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr2, result_address.Value, 1, false, ref_result_address);
        }
    }
}
