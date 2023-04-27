using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language
{
    public class Type : Named
    {
        public required int Size { get; init; }

        public override bool Equals(object? obj) => obj is Type t && Name == t.Name;
        public override int GetHashCode() => Name.GetHashCode();

        public override void Validate()
        {
            base.Validate();

            if (Size < 0)
            {
                throw new Exception("Size can not be smaller than 0.");
            }
        }
    }
}
