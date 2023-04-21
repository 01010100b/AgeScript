using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Language
{
    internal static class Primitives
    {
        public static Type Int => Types.Single(x => x.Name == "Int");
        public static Type Bool => Types.Single(x => x.Name == "Bool");
        public static Type Void => Types.Single(x => x.Name == "Void");

        public static IEnumerable<string> Keywords { get; } = new List<string>()
        {
            "Struct", "Globals", "Function", "Lookup", "return", "break", "continue", "true", "false",
            "if", "else", "elif", "endif", "for", "endfor", "while", "endwhile", "switch", "case", "endswitch"
        };

        public static IEnumerable<Type> Types { get; }  = new List<Type>()
        {
            new() { Name = "Int", Size = 1 },
            new() { Name = "Bool", Size = 1 },
            new() { Name = "Void", Size = 0 }
        };
    }
}
