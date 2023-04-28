using AgeScript.Compiler.Language;
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
    internal class ChatDataToSelf : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public ChatDataToSelf() : base()
        {
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "value", Type = Primitives.Int });
        }

        internal override void CompileCall2(CompilationResult result, CallExpression cl, int? result_address = null, bool ref_result_address = false)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            ExpressionCompiler2.Compile(result, cl.Arguments[0], result.Memory.Intr0);
            result.Rules.AddAction($"up-chat-data-to-self {cl.Literal} g: {result.Memory.Intr0}");
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
