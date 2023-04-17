using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal class RuleList
    {
        public int MaxElementsPerRule { get; set; } = 16;
        public Dictionary<string, int> Constants { get; } = new();
        public int CurrentRuleIndex => Rules.Count;

        private List<string> Rules { get; } = new();
        private StringBuilder CurrentRule { get; } = new();
        private int CurrentRuleElements { get; set; } = 0;

        public RuleList()
        {
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

            if (CurrentRuleElements >= MaxElementsPerRule)
            {
                StartNewRule();
            }
        }

        public void ReplaceStrings(string from, string to)
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                var current = Rules[i];
                Rules[i] = current.Replace(from, to);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var constant in Constants)
            {
                sb.AppendLine($"(defconst {constant.Key} {constant.Value})");
            }

            foreach (var rule in Rules)
            {
                sb.AppendLine();
                sb.AppendLine(rule);
            }

            return sb.ToString();
        }
    }
}
