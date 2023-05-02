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
        static void Main(string[] args)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            var settings = LoadJson<Settings>(file) ?? new Settings();
            WriteJson(settings, file);

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
            else if (string.IsNullOrWhiteSpace(settings.WorkingFolder) || !Directory.Exists(settings.WorkingFolder))
            {
                Console.WriteLine($"Can not find working folder.");

                return;
            }
            else if (string.IsNullOrWhiteSpace(settings.DestinationFolder) || !Directory.Exists(settings.DestinationFolder))
            {
                Console.WriteLine($"Can not find destination folder.");

                return;
            }

            if (args.Length > 0)
            {
                if (args[0].Trim() == "-compile")
                {
                    RunCompile(settings);
                }
                else if (args[0].Trim() == "-optimize")
                {
                    RunOptimize(settings);
                }
                else if (args[0].Trim() == "-link")
                {
                    RunLink(settings);
                }
                else if (args[0].Trim() == "-full")
                {
                    RunFull(settings);
                }
                else
                {
                    throw new Exception("Argument not recognized.");
                }
            }
            else
            {
                RunFull(settings);
            }
        }

        private static void RunCompile(Settings settings)
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

            File.WriteAllText(Path.Combine(settings.WorkingFolder, $"{settings.Name}.jtp"), jtp);
            WriteJson(targets, Path.Combine(settings.WorkingFolder, $"{settings.Name}.jt"));
        }

        private static void RunOptimize(Settings settings)
        {
            var file_jtp = Path.Combine(settings.WorkingFolder, $"{settings.Name}.jtp");
            var file_jt = Path.Combine(settings.WorkingFolder, $"{settings.Name}.jt");
            var jtp = File.ReadAllText(file_jtp);
            var jt = LoadJson<Dictionary<string, int>>(file_jt);
            
            if (jt is null)
            {
                throw new Exception("Failed to load files.");
            }

            var optimizer = new ScriptOptimizer();
            optimizer.Optimize(ref jtp, ref jt);

            File.Delete(file_jtp);
            File.WriteAllText(file_jtp, jtp);
            WriteJson(jt, file_jt);
        }

        private static void RunLink(Settings settings)
        {
            var file_jtp = Path.Combine(settings.WorkingFolder, $"{settings.Name}.jtp");
            var file_jt = Path.Combine(settings.WorkingFolder, $"{settings.Name}.jt");
            var file_per = Path.Combine(settings.DestinationFolder, $"{settings.Name}.per");

            var jtp = File.ReadAllText(file_jtp);
            var jt = LoadJson<Dictionary<string, int>>(file_jt);

            foreach (var jump in jt)
            {
                jtp = jtp.Replace(jump.Key, jump.Value.ToString());
            }

            File.Delete(file_per);
            File.WriteAllText(file_per, jtp);
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

        private static T? LoadJson<T>(string file)
        {
            if (File.Exists(file))
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(file));
            }
            else
            {
                return default;
            }
        }

        private static void WriteJson<T>(T obj, string file)
        {
            File.Delete(file);
            var options = new JsonSerializerOptions() { WriteIndented = true };
            File.WriteAllText(file, JsonSerializer.Serialize(obj, options));
        }
    }
}