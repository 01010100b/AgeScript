﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Intrinsics.DUC
{
    internal class RemoveObjects : Inlined
    {
        public override bool HasStringLiteral => true;

        public RemoveObjects() : base()
        {
            ReturnType = Primitives.Int4;
            Parameters.Add(new() { Name = "search_source", Type = Primitives.Int });
            Parameters.Add(new() { Name = "object_data", Type = Primitives.Int });
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
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

            var op = cl.Literal;
            ExpressionCompiler.Compile(result, cl.Arguments[2], result.Memory.Intr0);
            result.Rules.AddAction($"up-remove-objects {ce0.Int} {ce1.Int} g:{op} {result.Memory.Intr0}");

            if (result_address is not null)
            {
                result.Rules.AddAction($"up-get-search-state {result.Memory.Intr0}");
                Utils.MemCopy(result, result.Memory.Intr0, result_address.Value, 4, false, ref_result_address);
            }
        }
    }
}
