﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class CanBuildLine : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public CanBuildLine() : base()
        {
            ReturnType = Primitives.Bool;
            Parameters.Add(new() { Name = "escrow", Type = Primitives.Bool });
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "building_id", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[2], script.Intr3);
            rules.AddAction($"set-goal {script.Intr4} 0");

            rules.StartNewRule($"up-can-build-line {script.Intr0} {script.Intr1} g: {script.Intr3}");
            rules.AddAction($"set-goal {script.Intr4} 1");

            rules.StartNewRule();
            Utils.MemCopy(script, rules, script.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            ExpressionCompiler2.Compile(result, cl.Arguments[2], result.Memory.Intr3);
            result.Rules.AddAction($"set-goal {result.Memory.Intr4} 0");

            result.Rules.StartNewRule($"up-can-build-line {result.Memory.Intr0} {result.Memory.Intr1} g: {result.Memory.Intr3}");
            result.Rules.AddAction($"set-goal {result.Memory.Intr4} 1");

            result.Rules.StartNewRule();
            Utils.MemCopy2(result, result.Memory.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
}
