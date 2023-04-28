using AgeScript.Compiler.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler.Compilation
{
    public class ScriptCompiler
    {
        internal static Settings Settings { get; private set; } = new();

        private static readonly object Lock = new();

        public CompilationResult Compile2(Script script, Settings settings)
        {
            script.Validate();
            settings.Validate();
            var result = new CompilationResult(script, settings);

            PreCompile(result);
            CompileFunctions(result);
            CompileTables(result);
            PostCompile(result);

            result.Validate();

            return result;
        }

        private void PreCompile(CompilationResult result)
        {
            // don't run if error

            result.Rules.StartNewRule($"up-compare-goal {result.Memory.Error} c:> 0");
            result.Rules.AddAction($"up-chat-data-to-self \"ERROR: %d\" g: {result.Memory.Error}");
            result.Rules.AddAction($"up-jump-direct c: {result.Rules.EndTarget}");
            result.Rules.StartNewRule();

            if (result.Settings.Debug)
            {
                result.Rules.AddAction($"set-goal {result.Memory.Intr0} {result.Memory.StackLimit}");
                result.Rules.AddAction($"up-modify-goal {result.Memory.Intr0} g:- {result.Memory.DebugMaxStackSpaceUsed}");
                result.Rules.AddAction($"up-chat-data-to-self \"stack remaining: %d\" g: {result.Memory.Intr0}");
            }
        }

        private void CompileFunctions(CompilationResult result)
        {
            var function_compiler = new FunctionCompiler2();
            var functions = result.Rules.GetFunctions().ToList();
            functions.Sort((a, b) =>
            {
                if (a.Name == "Main")
                {
                    return -1;
                }
                else if (b.Name == "Main")
                {
                    return 1;
                }
                else
                {
                    return a.Name.CompareTo(b.Name);
                }
            });

            foreach (var function in functions)
            {
                function_compiler.Compile(result, function);
            }
        }

        private void CompileTables(CompilationResult result)
        {
            var table_compiler = new TableCompiler2();
            var tables = result.Rules.GetTables().ToList();
            tables.Sort((a, b) => a.Name.CompareTo(b.Name));

            foreach (var table in tables)
            {
                table_compiler.Compile(result, table);
            }
        }

        private void PostCompile(CompilationResult result)
        {
            Utils.CompileMemCopy2(result);
        }

        public RuleList Compile(Script script, Settings settings)
        {
            script.Validate();
            var rules = new RuleList(script, settings);

            lock (Lock)
            {
                Settings = settings;
                Compile(script, rules, settings);
            }

            return rules;
        }

        private void Compile(Script script, RuleList rules, Settings settings)
        {
            // layout memory

            var memory_compiler = new MemoryCompiler();
            memory_compiler.Compile(script, rules);

            // compile code

            var function_compiler = new FunctionCompiler();

            var functions = script.Functions.ToList();
            functions.Sort((a, b) =>
            {
                if (a.Name == "Main")
                {
                    return -1;
                }
                else if (b.Name == "Main")
                {
                    return 1;
                }
                else
                {
                    return a.Name.CompareTo(b.Name);
                }
            });

            foreach (var function in functions)
            {
                function_compiler.Compile(script, function, rules);
            }

            var table_compiler = new TableCompiler();
            var tables = script.Tables.ToList();
            tables.Sort((a, b) => a.Name.CompareTo(b.Name));

            foreach (var table in tables)
            {
                table_compiler.Compile(script, rules, table);
            }

            if (Settings.InlineMemCopy)
            {
                Utils.CompileMemCopy(script, rules);
            }

            // linker

            foreach (var function in functions)
            {
                rules.ReplaceStrings(function.AddressableName, function.Address.ToString());
            }

            foreach (var table in script.Tables)
            {
                rules.ReplaceStrings(table.AddressableName, table.Address.ToString());
            }
        }
    }
}
