using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgeScript.Language;
using AgeScript.Language.Expressions;

namespace AgeScript.Language.Statements
{
    public class AssignStatement : Statement
    {
        public required Accessor? Accessor { get; init; }
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            Accessor?.Validate();
            Expression.Validate();

            if (Accessor is not null && Expression is not CallExpression)
            {
                if (Accessor.Type != Expression.Type)
                {
                    throw new Exception("Expression type must match accessor type.");
                }
            }
        }
    }
}
