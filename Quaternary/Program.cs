using AgeScript.Compiler;
using AgeScript.Parser;
using AgeScript.Linker;

namespace Quaternary
{
    internal class Program
    {
        private static string Folder => AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            var settings = new Settings()
            {
                MaxElementsPerRule = 16,
                MaxGoal = 512,
                OptimizeMemCopy = true,
                InlineMemCopy = false,
                Debug = false
            };

            var name = "Quaternary";
            var source = @"F:\Repos\AgeScript\Quaternary\Source";
            var destination = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";

            Run(name, source, destination, settings);
        }

        private static void Run(string name, string source, string destination, Settings settings)
        {
            if (!Directory.Exists(source))
            {
                Console.WriteLine($"Can not find source folder.");

                return;
            }
            else if (!Directory.Exists(destination))
            {
                Console.WriteLine($"Can not find destination folder.");

                return;
            }

            var dirs = new List<string>() { source };

            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
            {
                dirs.Add(dir);
            }

            var lines = new List<string>();

            foreach (var dir in dirs)
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    Console.WriteLine($"Found source file: {file}");
                    lines.AddRange(File.ReadAllLines(file));
                }
            }

            var parser = new ScriptParser();
            var script = parser.Parse(lines);
            var compiler = new ScriptCompiler();
            var result = compiler.Compile(script, settings);
            var linker = new Linker();
            var code = linker.Link(result.JumpTargetPer, result.JumpTargets);

            var ai = Path.Combine(destination, $"{name}.ai");
            if (!File.Exists(ai))
            {
                File.Create(ai);
            }

            var per = Path.Combine(destination, $"{name}.per");
            File.Delete(per);
            File.WriteAllText(per, code);

            Console.WriteLine($"Compiled {name} succesfully.");
            Console.WriteLine($"Used {result.RuleCount:N0} rules and {result.CommandCount:N0} elements for {result.CommandCount / (double)result.RuleCount:N2} elements per rule.");
        }
    }
}