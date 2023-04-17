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
        public ChatDataToSelf(Script script) : base(script)
        {
            Name = "ChatDataToSelf";
            ReturnType = script.Types["Void"];
            Parameters.Add(new() { Name = "goal", Type = script.Types["Int"] });
        }

        public override void Compile(RuleList rules, CallExpression cl)
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
