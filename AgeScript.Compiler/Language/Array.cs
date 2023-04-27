using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language
{
    public class Array : Type
    {
        public required Type ElementType { get; init; }
        public required int Length { get; init; }

        public override void Validate()
        {
            ElementType.Validate();

            if (ElementType.Size == 0)
            {
                throw new Exception("Can not have arrays with an element type of size 0 (such as Void)");
            }

            if (Length <= 0)
            {
                throw new Exception("Array length must be greater than zero.");
            }

            if (Size != Length * ElementType.Size)
            {
                throw new Exception("Size must be equal to the element type's size * length.");
            }

            if (Name != $"{ElementType.Name}[{Length}]")
            {
                throw new Exception("Name does not follow rules.");
            }
        }
    }
}
