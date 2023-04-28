using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Statements
{
    public class ElifStatement : Statement
    {
        public required Expression Condition { get; init; }

        public override void Validate()
        {
            Condition.Validate();

            if (Condition.Type != Primitives.Bool)
            {
                throw new Exception("Elif statement needs expression of type Bool.");
            }
        }
    }
}
