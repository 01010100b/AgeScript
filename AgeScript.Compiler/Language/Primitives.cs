using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler.Language
{
    public static class Primitives
    {
        public static Type Void => Types.Single(x => x.Name == "Void");
        public static Type Bool => Types.Single(x => x.Name == "Bool");
        public static Type Int => Types.Single(x => x.Name == "Int");
        public static Type Precise => Types.Single(x => x.Name == "Precise");
        public static Array Int2 => (Array)Types.Single(x => x.Name == "Int[2]");
        public static Array Precise2 => (Array)Types.Single(x => x.Name == "Precise[2]");
        public static Array Int4 => (Array)Types.Single(x => x.Name == "Int[4]");

        public static IEnumerable<string> Keywords { get; } = new List<string>()
        {
            "Struct", "Globals", "Function", "Table", "String", "return", "break", "continue", "true", "false",
            "if", "else", "elif", "endif", "for", "endfor", "while", "endwhile", "switch", "case", "default", "endswitch"
        };

        public static IEnumerable<Type> Types { get; } = GetTypes();

        private static List<Type> GetTypes()
        {
            var types = new List<Type>()
            {
                new() { Name = "Void", Size = 0 },
                new() { Name = "Bool", Size = 1 },
                new() { Name = "Int", Size = 1 },
                new() { Name = "Precise", Size = 1}
            };

            types.Add(new Array()
            {
                Name = "Int[2]",
                ElementType = types.Single(x => x.Name == "Int"),
                Length = 2,
                Size = 2
            });

            types.Add(new Array()
            {
                Name = "Precise[2]",
                ElementType = types.Single(x => x.Name == "Precise"),
                Length = 2,
                Size = 2
            });

            types.Add(new Array()
            {
                Name = "Int[4]",
                ElementType = types.Single(x => x.Name == "Int"),
                Length = 4,
                Size = 4
            });

            return types;
        }
    }
}
