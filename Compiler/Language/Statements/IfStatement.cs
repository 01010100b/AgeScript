using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Statements
{
    internal class IfStatement : Statement
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
