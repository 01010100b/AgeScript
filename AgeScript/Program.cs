using AgeScript.Compiler;
using AgeScript.Linker;
using AgeScript.Optimizer;
using AgeScript.Parser;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgeScript
{
    internal class Program
    {
        private static string SettingsFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

        static void Main(string[] args)
        {
            var settings = LoadSettings();
            SaveSettings(settings);

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                Console.WriteLine($"Invalid name.");

                return;
            }
            else if (string.IsNullOrWhiteSpace(settings.SourceFolder) || !Directory.Exists(settings.SourceFolder))
            {
                Console.WriteLine($"Can not find source folder.");

                return;
            }
            else if (string.IsNullOrWhiteSpace(settings.DestinationFolder) || !Directory.Exists(settings.DestinationFolder))
            {
                Console.WriteLine($"Can not find destination folder.");

                return;
            }

            RunFull(settings);
        }

        private static void RunFull(Settings settings)
        {
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
            var result = compiler.Compile(script, settings.CompilerSettings);
            Console.WriteLine($"Compiled {settings.Name} succesfully.");
            Console.WriteLine($"Used {result.RuleCount:N0} rules and {result.CommandCount:N0} elements for {result.CommandCount / (double)result.RuleCount:N2} elements per rule.");

            var jtp = result.JumpTargetPer;
            var targets = new Dictionary<string, int>(result.JumpTargets);
            var optimizer = new ScriptOptimizer();
            optimizer.Optimize(ref jtp, ref targets);
            var linker = new ScriptLinker();
            var code = linker.Link(jtp, targets);

            var ai = Path.Combine(settings.DestinationFolder, $"{settings.Name}.ai");
            if (!File.Exists(ai))
            {
                File.Create(ai);
            }

            var per = Path.Combine(settings.DestinationFolder, $"{settings.Name}.per");
            File.Delete(per);
            File.WriteAllText(per, code);
        }

        private static Settings LoadSettings()
        {
            var settings = new Settings();

            if (File.Exists(SettingsFile))
            {
                var s = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFile));

                if (s is not null)
                {
                    settings = s;
                }
            }

            return settings;
        }

        private static void SaveSettings(Settings settings)
        {
            File.Delete(SettingsFile);
            var options = new JsonSerializerOptions() { WriteIndented = true };
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings, options));
        }
    }
}