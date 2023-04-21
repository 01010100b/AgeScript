﻿using Compiler.Compilation;
using Compiler.Language;
using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation.Intrinsics
{
    internal class Add : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public Add()
        {
            Name = "Add";
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "a", Type = Primitives.Int });
            Parameters.Add(new() { Name = "b", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules
            , CallExpression cl, int? address, bool ref_result_address)
        {
            if (address is null)
            {
                return;
            }

            rules.AddAction($"set-goal {script.Intr0} 0");

            foreach (var arg in cl.Arguments)
            {
                ExpressionCompiler.Compile(script, function, rules, arg, script.Intr1);
                rules.AddAction($"up-modify-goal {script.Intr0} g:+ {script.Intr1}");
            }

            rules.AddAction($"up-modify-goal {address} g:= {script.Intr0}");
        }
    }
}
