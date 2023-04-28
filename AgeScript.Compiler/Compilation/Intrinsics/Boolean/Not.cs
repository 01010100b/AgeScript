﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.Boolean
{
    internal class Not : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Not() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "a", Type = Primitives.Bool });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"set-goal {result.Memory.Intr1} 0");
            result.Rules.StartNewRule($"not (goal {result.Memory.Intr0} 1)");
            result.Rules.AddAction($"set-goal {result.Memory.Intr1} 1");
            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr1, result_address.Value, 1, false, ref_result_address);
        }
    }
}
