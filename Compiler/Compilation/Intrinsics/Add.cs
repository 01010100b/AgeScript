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
        public Add(Script script) : base(script)
        {
            Name = "Add";
            ReturnType = script.Types["Int"];
            Parameters.Add(new() { Name = "a", Type = ReturnType });
            Parameters.Add(new() { Name = "b", Type = ReturnType });
        }

        public override void Compile(RuleList rules, CallExpression cl, int? address)
        {
            rules.AddAction($"set-goal sp0 0");

            foreach (var arg in cl.Arguments)
            {
                if (arg is VariableExpression ve)
                {
                    rules.AddAction($"up-modify-goal sp0 g:+ {ve.Variable.Address}");
                }
                else if (arg is ConstExpression ce)
                {
                    rules.AddAction($"up-modify-goal sp0 c:+ {int.Parse(ce.Value)}");
                }
            }

            if (address is not null)
            {
                rules.AddAction($"up-modify-goal {address.Value} g:= sp0");
            }
        }
    }
}
