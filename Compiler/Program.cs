using Compiler.Compilation;
using Compiler.Language;
using Compiler.Parsing;
using System.Text.Json;

namespace Compiler
{
    internal class Program
    {
        public static Settings Settings { get; private set; } = new();
        public static string Folder => AppDomain.CurrentDomain.BaseDirectory;
        public static string SourceFolder => Path.Combine(Folder, "Source");

        static void Main(string[] args)
        {
            var file = Path.Combine(Folder, "Settings.json");
            var options = new JsonSerializerOptions() { WriteIndented = true };

            if (File.Exists(file))
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(file));

                if (settings is not null)
                {
                    Settings = settings;
                }
            }

            Run();

            File.Delete(file);
            File.WriteAllText(file, JsonSerializer.Serialize(Settings, options));
        }

        private static void Run()
        {
            var dirs = new List<string>() { SourceFolder };
            var lines = new List<string>();


            foreach (var dir in Directory.EnumerateDirectories(SourceFolder, "*", SearchOption.AllDirectories))
            {
                dirs.Add(dir);
            }

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
            var rules = compiler.Compile(script);
            var code = rules.ToString();

            var ai = Path.Combine(Settings.Folder, $"{Settings.Name}.ai");
            File.Create(ai);
            var per = Path.Combine(Settings.Folder, $"{Settings.Name}.per");
            File.WriteAllText(per, code);

            File.WriteAllText(Path.Combine(Folder, "script debug.txt"), script.ToString());

            Console.WriteLine($"Compiled {Settings.Name} succesfully.");
            Console.WriteLine($"Used {rules.RuleCount:N0} rules and {rules.ElementsCount:N0} elements for {rules.ElementsCount / rules.RuleCount:N0} elements per rule");
        }
    }
}