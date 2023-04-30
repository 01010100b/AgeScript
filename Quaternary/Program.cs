using System.Text.Json;
using System.Diagnostics;
using AgeScript;

namespace Quaternary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Compile();
        }

        private static void Compile()
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            var settings = new Settings()
            {
                Name = "Quaternary",
                SourceFolder = @"F:\Repos\01010100b\AgeScript\Quaternary\Source",
                WorkingFolder = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai",
                DestinationFolder = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai",
                CompilerSettings = new()
            };

            File.Delete(file);
            var options = new JsonSerializerOptions() { WriteIndented = true };
            File.WriteAllText(file, JsonSerializer.Serialize(settings, options));

            var tool = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AgeScript.exe");
            var process = Process.Start(tool);
            process.WaitForExit();
        }
    }
}