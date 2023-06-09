﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Intrinsics
{
    internal class ChatDataToSelf : Inlined
    {
        public override bool HasStringLiteral => true;

        public ChatDataToSelf() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            ExpressionCompiler.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-chat-data-to-self \"{cl.Literal}\" g: {result.Memory.Intr0}");
        }
    }

    internal class ChatDataToSelfPrecise : ChatDataToSelf
    {
        public ChatDataToSelfPrecise() : base()
        {
            Name = "ChatDataToSelf";
            Parameters.Clear();
            Parameters.Add(new() { Name = "value", Type = Primitives.Precise });
        }
    }
}
