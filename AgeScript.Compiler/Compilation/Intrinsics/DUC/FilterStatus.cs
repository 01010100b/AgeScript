﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics.DUC
{
    internal class FilterStatus : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public FilterStatus() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "object_status", Type = Primitives.Int });
            Parameters.Add(new() { Name = "object_list", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"up-filter-status g: {script.Intr0} g: {script.Intr1}");
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr1);
            result.Rules.AddAction($"up-filter-status g: {result.Memory.Intr0} g: {result.Memory.Intr1}");
        }
    }
}
