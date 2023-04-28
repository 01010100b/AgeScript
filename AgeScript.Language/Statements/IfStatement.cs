using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Statements
{
    public class IfStatement : Statement
    {
        public required Expression Condition { get; init; }

        public override void Validate()
        {
            Condition.Validate();

            if (Condition.Type != Primitives.Bool)
            {
                throw new Exception("If statement needs expression of type Bool.");
            }
        }
    }
}
