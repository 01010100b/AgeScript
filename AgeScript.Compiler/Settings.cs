using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    public class Settings
    {
        public int MaxGoal { get; set; } = 512;
        public int MaxElementsPerRule { get; set; } = 16;
        public bool OptimizeMemCopy { get; set; } = true;
        public bool InlineMemCopy { get; set; } = false;
        public bool Debug { get; set; } = false;
        [JsonIgnore]
        public int TableModulus => MaxElementsPerRule - 2;

        public void Validate()
        {

        }
    }
}