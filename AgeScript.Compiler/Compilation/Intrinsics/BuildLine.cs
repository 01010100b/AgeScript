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
    internal class BuildLine : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public BuildLine() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "point", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "building_id", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr2);
            rules.StartNewRule($"up-can-build-line 0 {script.Intr0} g: {script.Intr2}");
            rules.AddAction($"up-build-line {script.Intr0} {script.Intr0} g: {script.Intr2}");
            rules.StartNewRule();
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            ExpressionCompiler2.Compile(result, cl.Arguments[1], result.Memory.Intr2);
            result.Rules.StartNewRule($"up-can-build-line 0 {result.Memory.Intr0} g: {result.Memory.Intr2}");
            result.Rules.AddAction($"up-build-line {result.Memory.Intr0} {result.Memory.Intr0} g: {result.Memory.Intr2}");
            result.Rules.StartNewRule();
        }
    }
}
