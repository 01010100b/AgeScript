﻿using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics.DUC
{
    internal class RemoveObjects : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public RemoveObjects() : base()
        {
            Parameters.Add(new() { Name = "search_source", Type = Primitives.Int });
            Parameters.Add(new() { Name = "object_data", Type = Primitives.Int });
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("literal is null.");
            }

            if (cl.Arguments[0] is not ConstExpression ce0)
            {
                throw new Exception("search_source must be const expression.");
            }

            if (cl.Arguments[1] is not ConstExpression ce1)
            {
                throw new Exception("object_data must be const expression.");
            }

            var op = cl.Literal.Replace("\"", string.Empty);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[2], script.Intr0);
            rules.AddAction($"up-remove-objects {ce0.Int} {ce1.Int} g:{op} {script.Intr0}");
        }
    }
}
