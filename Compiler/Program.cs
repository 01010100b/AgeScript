using Compiler.Compilation;
using Compiler.Language;
using Compiler.Parsing;
using System.Text.Json;

namespace Compiler
{
    internal class Program
    {
        public static string Folder => AppDomain.CurrentDomain.BaseDirectory;
        public static string SourceFolder => Path.Combine(Folder, "Source");

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

            if (Directory.Exists(settings.Folder))
            {
                Run(settings);
            }
            else
            {
                Console.WriteLine($"Output dir {settings.Folder} not found.");
            }

            File.Delete(file);
            File.WriteAllText(file, JsonSerializer.Serialize(settings, options));
        }

        private static void Run(Settings settings)
        {
            var dirs = new List<string>() { SourceFolder };

            foreach (var dir in Directory.EnumerateDirectories(SourceFolder, "*", SearchOption.AllDirectories))
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

            var ai = Path.Combine(settings.Folder, $"{settings.Name}.ai");

            if (!File.Exists(ai))
            {
                File.Create(ai);
            }
            
            var per = Path.Combine(settings.Folder, $"{settings.Name}.per");
            File.Delete(per);
            File.WriteAllText(per, code);

            File.WriteAllText(Path.Combine(Folder, "script debug.json"), script.ToString());

            Console.WriteLine($"Compiled {settings.Name} succesfully.");
            Console.WriteLine($"Used {rules.RuleCount:N0} rules and {rules.ElementsCount:N0} elements for {rules.ElementsCount / rules.RuleCount:N0} elements per rule.");
        }
    }
}