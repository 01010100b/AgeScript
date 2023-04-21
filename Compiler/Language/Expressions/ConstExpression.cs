using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Expressions
{
    internal class ConstExpression : Expression
    {
        public static ConstExpression Zero { get; } = new("0");
        public static ConstExpression False { get; } = new("false");
        public static ConstExpression True { get; } = new("true");

        public override Type Type => ConstType;
        public int Int { get; private init; } = 0;
        public bool Bool { get; private init; } = false;

        private Type ConstType { get; init; }

        public ConstExpression(string value) 
        {
            if (int.TryParse(value, out var i))
            {
                ConstType = Primitives.Int;
                Int = i;
            }
            else if (bool.TryParse(value, out var b))
            {
                ConstType = Primitives.Bool;
                Bool = b;
            }
            else
            {
                throw new Exception($"Invalid const: {value}");
            }
        }

        public override void Validate()
        {
        }
    }
}
