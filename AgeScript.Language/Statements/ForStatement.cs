using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Statements
{
    public class ForStatement : Statement
    {
        public required Variable Variable { get; init; }
        public required Expression From { get; init; }
        public required Expression To { get; init; }

        public override void Validate()
        {
            Variable.Validate();
            From.Validate();
            To.Validate();

            if (Variable.Type != Primitives.Int)
            {
                throw new Exception("For loop variable must have type Int.");
            }
            else if (From.Type != Primitives.Int)
            {
                throw new Exception("For loop from must have type Int.");
            }
            else if (To.Type != Primitives.Int)
            {
                throw new Exception("For loop to must have type Int.");
            }
        }
    }
}
