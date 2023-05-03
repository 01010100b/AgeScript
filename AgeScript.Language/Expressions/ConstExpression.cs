using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Expressions
{
    public class ConstExpression : Expression
    {
        public static ConstExpression Zero { get; } = FromInt(0);
        public static ConstExpression False { get; } = FromBool(false);
        public static ConstExpression True { get; } = FromBool(true);
        public static ConstExpression FromInt(int value) => new(Primitives.Int) { Int = value };
        public static ConstExpression FromBool(bool value) => new(Primitives.Bool) { Bool = value };
        public static ConstExpression FromPrecise(float value) => new(Primitives.Precise) { Precise = (int)Math.Round(value * 100) };

        public override Type Type => ConstType;
        public int Int { get; private init; } = 0;
        public bool Bool { get; private init; } = false;
        public int Precise { get; private init; } = 0;

        private Type ConstType { get; init; }

        private ConstExpression(Type type)
        {
            ConstType = type;
        }

        public override void Validate()
        {
        }
    }
}
