using Compiler.Compilation;
using Compiler.Language;
using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation.Intrinsics
{
    internal class ChatDataToSelf : Intrinsic
    {
        public override bool HasStringLiteral => true;

        public ChatDataToSelf()
        {
            Name = "ChatDataToSelf";
            ReturnType = Primitives.Void;
            Parameters.Add(new() { Name = "goal", Type = Primitives.Int });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, 
            CallExpression cl, int? address, bool ref_result_address)
        {
            if (cl.Arguments[0] is VariableExpression ve)
            {
                if (cl.Literal is null)
                {
                    throw new Exception("Literal is null.");
                }

                rules.AddAction($"up-chat-data-to-self {cl.Literal} g: {ve.Variable.Address}");
            }
            else
            {
                throw new Exception("Argument must be variable expression.");
            }
        }
    }
}
