using AgeScript;
using System.Diagnostics;
using System.Text.Json;

namespace Deimos
{
    internal class Program
    {
        private const string DE_FOLDER = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";
        private const string WK_FOLDER = @"F:\AoE\WK\Voobly Mods\AOC\Data Mods\WololoKingdoms\Script.Ai";

        static void Main(string[] args)
        {
            //Stuff.Run();
            //return;
            var settings = new Settings()
            {
                Name = "Deimos",
                SourceFolder = @"F:\Repos\01010100b\AgeScript\Deimos\Source",
                WorkingFolder = WK_FOLDER,
                DestinationFolder = WK_FOLDER,
                CompilerSettings = new()
            };

            Compile(settings);

            settings.WorkingFolder = DE_FOLDER;
            settings.DestinationFolder = DE_FOLDER;
            Compile(settings);
        }

        private static void Compile(Settings settings)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            File.Delete(file);
            var options = new JsonSerializerOptions() { WriteIndented = true };
            File.WriteAllText(file, JsonSerializer.Serialize(settings, options));

            var tool = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AgeScript.exe");
            var process = Process.Start(tool, "-full");
            process.WaitForExit();
        }
    }
}