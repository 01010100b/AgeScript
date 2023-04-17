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
        public int RegisterBase { get; set; }
        public int CallResultBase { get; set; }

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
