using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compilation
{
    public class RuleList
    {
        public int RuleCount => Rules.Count;
        public int ElementsCount => Rules.Sum(GetElementsCount);

        internal int CurrentRuleIndex => Rules.Count;

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

            if (CurrentRuleElements >= ScriptCompiler.Settings.MaxElementsPerRule)
            {
                StartNewRule();
            }
        }

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

        private int GetElementsCount(string rule) => rule.Count(x => x == '(') - 1;
    }
}
