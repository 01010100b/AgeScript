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
        public required Accessor? Accessor { get; init; }
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            Accessor?.Validate();
            Expression.Validate();
        }
    }
}
