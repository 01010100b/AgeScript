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
        public string Folder { get; set; } = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";
        public int MaxGoal { get; set; } = 512;
        public int MaxElementsPerRule { get; set; } = 16;
    }
}
