using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Settings
    {
        public string Name { get; set; } = "Example";
        public string SourceFolder { get; set; } = @"F:\Repos\AgeScript\Compiler\Source";
        public string DestinationFolder { get; set; } = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";
        public AgeScript.Settings CompilerSettings { get; set; } = new();
    }
}
