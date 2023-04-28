using AgeScript.Compiler.Intrinsics;
using AgeScript.Language;
using AgeScript.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgeScript.Compiler
{
    public class RuleList
    {
        public int RuleCount => Rules.Count;
        public int ElementsCount => Rules.Sum(x => x.Count(y => y == '(') - 1);

        internal string EndTarget { get; private init; }
        internal string MemCopyTarget { get; private init; }

        internal int CurrentRuleIndex => Rules.Count;

        private List<string> Rules { get; } = new();
        private StringBuilder CurrentRule { get; } = new();
        private int CurrentRuleElements { get; set; } = 0;
        private Dictionary<string, int> JumpTargets { get; } = new();
        private Dictionary<Function, string> FunctionTargets { get; } = new();
        private Dictionary<Table, string> TableTargets { get; } = new();
        private Settings Settings { get; init; }

        public RuleList(Script script, Settings settings)
        {
            Settings = settings;

            foreach (var intrinsic in Intrinsic.Intrinsics)
            {
                if (FunctionTargets.ContainsKey(intrinsic))
                {
                    throw new Exception("Intrinsic already exists.");
                }

                FunctionTargets.Add(intrinsic, string.Empty);
            }

            foreach (var function in script.Functions)
            {
                if (FunctionTargets.ContainsKey(function))
                {
                    throw new Exception("Function already exists.");
                }

                var target = CreateJumpTarget();
                FunctionTargets.Add(function, target);
            }

            foreach (var table in script.Tables)
            {
                if (TableTargets.ContainsKey(table))
                {
                    throw new Exception("Table already exists.");
                }

                var target = CreateJumpTarget();
                TableTargets.Add(table, target);
            }

            EndTarget = CreateJumpTarget();
            ResolveJumpTarget(EndTarget, 20000);
            if (!Settings.InlineMemCopy)
            {
                MemCopyTarget = CreateJumpTarget();
            }
            else
            {
                MemCopyTarget = "UNUSED MEM COPY TARGET";
            }

            StartNewRule();
        }

        public void StartNewRule(string condition = "true")
        {
            if (CurrentRuleElements > 1)
            {
                CurrentRule.AppendLine(")");
                Rules.Add(CurrentRule.ToString());
            }

            CurrentRule.Clear();
            CurrentRule.AppendLine("(defrule");
            CurrentRule.AppendLine($"\t({condition})");
            CurrentRule.AppendLine("=>");
            CurrentRuleElements = 1;
        }

        public void AddAction(string action)
        {
            CurrentRule.AppendLine($"\t({action})");
            CurrentRuleElements++;

            if (CurrentRuleElements >= Settings.MaxElementsPerRule)
            {
                StartNewRule();
            }
        }

        public string CreateJumpTarget()
        {
            var target = Guid.NewGuid().ToString().Replace("-", string.Empty);
            JumpTargets.Add(target, -1);

            return target;
        }

        public string GetJumpTarget(Function function)
        {
            if (FunctionTargets.TryGetValue(function, out var target))
            {
                return target;
            }
            else
            {
                throw new Exception("Function not found.");
            }
        }

        public string GetJumpTarget(Table table)
        {
            if (TableTargets.TryGetValue(table, out var target))
            {
                return target;
            }
            else
            {
                throw new Exception("Table not found.");
            }
        }

        public void ResolveJumpTarget(string target, int address)
        {
            if (!JumpTargets.ContainsKey(target))
            {
                throw new Exception("No such jump target.");
            }

            JumpTargets[target] = address;
        }

        public void ResolveJumpTarget(string target) => ResolveJumpTarget(target, CurrentRuleIndex);

        public Function GetFunction(string name, IReadOnlyList<Expression> arguments, string? literal)
        {
            try
            {
                return FunctionTargets.Keys.Single(x =>
                {
                    if (x.Name != name || x.Parameters.Count != arguments.Count)
                    {
                        return false;
                    }
                    else
                    {
                        if (x is Intrinsic intr)
                        {
                            if (intr.HasStringLiteral == literal is null)
                            {
                                return false;
                            }
                        }
                        else if (literal is not null)
                        {
                            return false;
                        }

                        for (int i = 0; i < arguments.Count; i++)
                        {
                            var a = arguments[i];
                            var p = x.Parameters[i];

                            if (a.Type != p.Type)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                });
            }
            catch
            {
                throw new Exception("Failed to resolve function.");
            }
        }

        public IEnumerable<Function> GetFunctions() => FunctionTargets.Keys;

        public Table GetTable(string name)
        {
            try
            {
                return TableTargets.Keys.Single(x => x.Name == name);
            }
            catch
            {
                throw new Exception("Table not found.");
            }
        }

        public IEnumerable<Table> GetTables() => TableTargets.Keys;

        public string GetPer()
        {
            return ToString();
        }

        public IReadOnlyDictionary<string, int> GetJumpTargets() => JumpTargets;

        public void ReplaceStrings(string from, string to)
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                Rules[i] = Rules[i].Replace(from, to);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"; Compiled with AgeScript v{GetType().Assembly.GetName().Version}");
            sb.AppendLine();

            for (int i = 0; i < Rules.Count; i++)
            {
                sb.AppendLine($"; {i}");
                sb.AppendLine(Rules[i]);
            }

            return sb.ToString();
        }
    }
}
