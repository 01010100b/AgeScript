using Compiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language.Statements
{
    internal class ElifStatement : Statement
    {
        public required Expression Expression { get; init; }

        public override void Validate()
        {
            if (Expression.Type.Name != "Bool")
            {
                throw new Exception("Elif statement needs expression of type Bool.");
            }
        }
    }
}
