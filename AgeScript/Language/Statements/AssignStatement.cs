using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Language.Expressions;

namespace AgeScript.Language.Statements
{
    public class AssignStatement : Statement
    {
        public required Variable? Variable { get; init; }
        public required int Offset { get; init; }
        public required Type Type { get; init; }
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            Expression.Validate();

            if (Offset < 0)
            {
                throw new Exception("Offset can not be smaller than 0.");
            }

            Variable?.Validate();
            Type.Validate();

            if (Variable is not null && !Type.Equals(Expression.Type))
            {
                throw new Exception("Type must match expression type.");
            }
        }
    }
}
