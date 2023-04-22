using Compiler.Compilation;
using Compiler.Language;
using Compiler.Parsing;
using System.Text.Json;

namespace Compiler
{
    internal class Program
    {
        private static string Folder => AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            var settings = new Settings();
            var file = Path.Combine(Folder, "Settings.json");
            var options = new JsonSerializerOptions() { WriteIndented = true };

            if (File.Exists(file))
            {
                var s = JsonSerializer.Deserialize<Settings>(File.ReadAllText(file));

                if (s is not null)
                {
                    settings = s;
                }
            }

            Run(settings);

            File.Delete(file);
            File.WriteAllText(file, JsonSerializer.Serialize(settings, options));
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