using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Expressions
{
    public class VariableExpression : Expression
    {
        public required Variable Variable { get; init; }
        public required int Offset { get; init; }
        public required Type ElementType { get; init; }
        public override Type Type => ElementType;

        public override void Validate()
        {
            Variable.Validate();
            ElementType.Validate();

            if (Offset < 0)
            {
                throw new Exception("Offset can not be smaller than 0.");
            }
        }
    }
}
