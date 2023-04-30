using AgeScript.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript
{
    public class Settings
    {
        public string Name { get; set; } = string.Empty;
        public string SourceFolder { get; set; } = string.Empty;
        public string WorkingFolder { get; set; } = string.Empty;
        public string DestinationFolder { get; set; } = string.Empty;
        public Compiler.Settings CompilerSettings { get; set; } = new();
    }
}
