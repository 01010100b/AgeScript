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

        internal int CondGoal { get; set; } // to hold result of control-flow (if, ...) conditions
        internal int StackPtr { get; set; } // points to next free stack goal
        internal int Sp0 { get; set; } // special purpose registers, for memcopy and such
        internal int Sp1 { get; set; }
        internal int Sp2 { get; set; }
        internal int Sp3 { get; set; }
        internal int Intr0 { get; set; } // special registers for intrinsics
        internal int Intr1 { get; set; }
        internal int Intr2 { get; set; }
        internal int Intr3 { get; set; }
        internal int RegisterBase { get; set; } // start of registers
        internal int RegisterCount { get; set; } // number of registers
        internal int CallResultBase { get; set; } // start of goals where result of a function call is stored

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

            foreach (var type in Types.Values)
            {
                type.Validate();
            }

            foreach (var global in GlobalVariables.Values)
            {
                global.Validate();
            }

            foreach (var function in Functions)
            {
                function.Validate();
            }
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
