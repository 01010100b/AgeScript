using AgeScript.Compiler.Language;
using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal override void CompileCall(Script script, Function function, RuleList rules,
            CallExpression cl, int? result_address, bool ref_result_address)
        {
            if (cl.Literal is null)
            {
                throw new Exception("Literal is null.");
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            rules.AddAction($"up-chat-data-to-self {cl.Literal} g: {script.Intr0}");
        }
    }

    internal class ChatDataToSelfPrecise : ChatDataToSelf
    {
        public ChatDataToSelfPrecise() : base()
        {
            Parameters.Clear();
            Parameters.Add(new() { Name = "value", Type = Primitives.Precise });
        }
    }
}
