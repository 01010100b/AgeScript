using AgeScript;
using AgeScript.Compilation;
using AgeScript.Parsing;

namespace Quaternary
{
    internal class Program
    {
        private static string Folder => AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            var settings = new Settings()
            {
                Name = "Quaternary",
                SourceFolder = @"F:\Repos\AgeScript\Quaternary\Source",
                DestinationFolder = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai",
                MaxElementsPerRule = 16,
                MaxGoal = 512,
                OptimizeMemCopy = true
            };

            Run(settings);
        }

        private static void Run(Settings settings)
        {
            if (!Directory.Exists(settings.SourceFolder))
            {
                Console.WriteLine($"Can not find source folder.");
            }
            else if (!Directory.Exists(settings.DestinationFolder))
            {
                Console.WriteLine($"Can not find destination folder.");
            }

            var dirs = new List<string>() { settings.SourceFolder };

            foreach (var dir in Directory.EnumerateDirectories(settings.SourceFolder, "*", SearchOption.AllDirectories))
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

            var ai = Path.Combine(settings.DestinationFolder, $"{settings.Name}.ai");
            if (!File.Exists(ai))
            {
                File.Create(ai);
            }

            var per = Path.Combine(settings.DestinationFolder, $"{settings.Name}.per");
            File.Delete(per);
            File.WriteAllText(per, code);

            File.WriteAllText(Path.Combine(Folder, "script debug.json"), script.ToString());

            Console.WriteLine($"Compiled {settings.Name} succesfully.");
            Console.WriteLine($"Used {rules.RuleCount:N0} rules and {rules.ElementsCount:N0} elements for {rules.ElementsCount / (double)rules.RuleCount:N2} elements per rule.");
        }
    }
}