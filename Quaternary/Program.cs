using AgeScript.Compiler;
using AgeScript.Compiler.Compilation;
using AgeScript.Compiler.Parsing;

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
                InlineMemCopy = true
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
            var rules = compiler.Compile(script, settings);
            var code = rules.ToString();

            var ai = Path.Combine(destination, $"{name}.ai");
            if (!File.Exists(ai))
            {
                File.Create(ai);
            }

            var per = Path.Combine(destination, $"{name}.per");
            File.Delete(per);
            File.WriteAllText(per, code);

            File.WriteAllText(Path.Combine(Folder, "script debug.json"), script.ToString());

            Console.WriteLine($"Compiled {name} succesfully.");
            Console.WriteLine($"Used {rules.RuleCount:N0} rules and {rules.ElementsCount:N0} elements for {rules.ElementsCount / (double)rules.RuleCount:N2} elements per rule.");
        }
    }
}