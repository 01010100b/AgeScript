using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language
{
    internal class Variable : Named
    {
        public required Type Type {  get; init; }

        internal int Address { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (Type == Primitives.Void)
            {
                throw new Exception("Can not have variable of type Void.");
            }

            Type.Validate();
        }
    }
}
