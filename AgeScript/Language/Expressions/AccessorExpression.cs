using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language.Expressions
{
    internal class AccessorExpression : Expression
    {
        public required Accessor Accessor { get; init; }
        public override Type Type => Accessor.Type;

        public override void Validate()
        {
            Accessor.Validate();
        }
    }
}
