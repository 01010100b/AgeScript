using Compiler.Compilation.Intrinsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Compiler.Language
{
    internal class Script : Validated
    {
        public Dictionary<string, Type> Types { get; } = new();
        public Dictionary<string, Variable> GlobalVariables { get; } = new();
        public List<Function> Functions { get; } = new();

        internal int CondGoal { get; set; }
        internal int StackPtr { get; set; }
        internal int Sp0 { get; set; }
        internal int Sp1 { get; set; }
        internal int Sp2 { get; set; }
        internal int Sp3 { get; set; }
        internal int Intr0 { get; set; }
        internal int Intr1 { get; set; }
        internal int Intr2 { get; set; }
        internal int Intr3 { get; set; }
        internal int RegisterBase { get; set; }
        internal int RegisterCount { get; set; }
        internal int CallResultBase { get; set; }

        public Script() 
        {
            foreach (var type in Primitives.Types)
            {
                Types.Add(type.Name, type);
            }

            foreach (var intrinsic in Intrinsic.GetIntrinsics(this))
            {
                Functions.Add(intrinsic);
            }
        }

        public override void Validate()
        {
            if (Functions.Count(x => x.Name == "Main") != 1)
            {
                throw new Exception("Must have a unique Main function.");
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
