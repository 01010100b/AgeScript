using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation.Intrinsics
{
    internal class GetPointDistance : Intrinsic
    {
        public override bool HasStringLiteral => false;

        public GetPointDistance() : base()
        {
            ReturnType = Primitives.Int;
            Parameters.Add(new() { Name = "point1", Type = Primitives.Int2 });
            Parameters.Add(new() { Name = "point2", Type = Primitives.Int2 });
        }

        internal override void CompileCall(Script script, Function function, RuleList rules, CallExpression cl, int? result_address, bool ref_result_address = false)
        {
            if (result_address is null)
            {
                return;
            }

            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[0], script.Intr0);
            ExpressionCompiler.Compile(script, function, rules, cl.Arguments[1], script.Intr2);
            rules.AddAction($"up-get-point-distance {script.Intr0} {script.Intr2} {script.Intr4}");
            Utils.MemCopy(script, rules, script.Intr4, result_address.Value, ReturnType.Size, false, ref_result_address);
        }
    }
    
    internal class GetPointDistancePrecise : GetPointDistance
    {
        public GetPointDistancePrecise() : base()
        {
            Name = "GetPointDistance";
            ReturnType = Primitives.Precise;
            Parameters.Clear();
            Parameters.Add(new() { Name = "point1", Type= Primitives.Precise2 });
            Parameters.Add(new() { Name = "point2", Type = Primitives.Precise2 });
        }
    }
}
