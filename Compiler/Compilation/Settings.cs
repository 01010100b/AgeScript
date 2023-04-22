using Compiler.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    public class Settings
    {
        public string Name { get; set; } = "Example";
        public string SourceFolder { get; set; } = @"F:\Repos\AgeScript\Compiler\Source";
        public string DestinationFolder { get; set; } = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";
        public int MaxGoal { get; set; } = 512;
        public int MaxElementsPerRule { get; set; } = 16;
        public bool OptimizeMemCopy { get; set; } = true; // 63-325 -> 28-178
    }
}
