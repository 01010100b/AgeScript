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
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            if (Expression.Type.Name != "Bool")
            {
                throw new Exception("If statement needs expression of type Bool.");
            }
        }
    }
}
