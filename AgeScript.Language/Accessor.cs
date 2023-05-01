using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Language
{
    public class Accessor : Validated
    {
        public required Variable Variable { get; init; }
        public required Expression Offset { get; init; }
        public required Type Type { get; init; }

        public override void Validate()
        {
            Variable.Validate();
            Offset.Validate();
            Type.Validate();

            if (Offset.Type != Primitives.Int)
            {
                throw new Exception("Offset must have type Int.");
            }

            if (Offset is not ConstExpression)
            {
                if (Offset is not AccessorExpression acc || acc.Accessor.Offset is not ConstExpression)
                {
                    throw new Exception("Can not nest accessors.");
                }
            }
        }
    }
}
