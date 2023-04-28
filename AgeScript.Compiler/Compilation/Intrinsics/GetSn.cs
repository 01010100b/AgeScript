﻿using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Compilation.Intrinsics
{
    internal class GetSn : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetSn() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "sn", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("sn must be const expression.");
            }

            rules.AddAction($"up-modify-goal {script.Intr0} s:= {ce.Int}");
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, 1, false, ref_result_address);
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            if (cl.Arguments[0] is not ConstExpression ce)
            {
                throw new Exception("sn must be const expression.");
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} s:= {ce.Int}");
            Utils.MemCopy2(result, result.Memory.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
