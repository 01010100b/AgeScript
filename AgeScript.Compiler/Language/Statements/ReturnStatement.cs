using AgeScript.Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language.Statements
{
    public class ReturnStatement : Statement
    {
        public required Expression? Expression { get; init; }

        public override void Validate()
        {
            Expression?.Validate();
        }
    }
}
