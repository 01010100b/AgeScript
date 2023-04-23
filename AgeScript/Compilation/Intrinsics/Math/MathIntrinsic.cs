using AgeScript.Language.Expressions;
using AgeScript.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = AgeScript.Language.Type;

namespace AgeScript.Compilation.Intrinsics.Math
{
    internal abstract class MathIntrinsic : Intrinsic
    {
        public override bool HasStringLiteral => false;
        protected abstract Type ParameterType { get; }

        public MathIntrinsic() : base()
        {
            Name = GetType().Name;
            ReturnType = ParameterType;
            Parameters.Add(new() { Name = "a", Type = ParameterType });
            Parameters.Add(new() { Name = "b", Type = ParameterType });
        }

        protected void CompileMath(Script script, Function function, RuleList rules, string op,
            CallExpression cl, int? result_address, bool ref_result_address)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr1);
            rules.AddAction($"up-modify-goal {script.Intr0} g:{op} {script.Intr1}");
            Utils.MemCopy(script, rules, script.Intr0, result_address.Value, 1, false, ref_result_address);
        }
    }
}
