using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Expressions
{
    internal class ConstExpression : Expression
    {
        public string Value { get; private init; }
        public override Type Type => ConstType;

        private Type ConstType { get; init; }

        public ConstExpression(string value) 
        {
            Value = value;

            if (int.TryParse(value, out _))
            {
                ConstType = Primitives.Types.Single(x => x.Name == "Int");
            }
            else if (bool.TryParse(value, out _))
            {
                ConstType = Primitives.Types.Single(x => x.Name == "Bool");
            }
            else
            {
                throw new Exception("Invalid const.");
            }
        }

        public override void Validate()
        {
        }
    }
}
