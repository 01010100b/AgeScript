using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    internal class PerParser
    {
        public List<Rule> Parse(string per)
        {
            var rules = new List<Rule>();
            var pieces = per.Split("(defrule").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var allow = true;

            for (int i = 1; i < pieces.Count; i++)
            {
                if (pieces[i - 1].Contains("#OPTIMIZER END"))
                {
                    allow = false;
                }

                var rule = ParseRule(pieces[i]);

                rule.AllowOptimizations = allow;
                if (rule.Actions.Select(x => x.Code).Contains("(disable-self)"))
                {
                    rule.AllowOptimizations = false;
                }

                rules.Add(rule);
            }

            return rules;
        }

        public string Write(List<Rule> rules)
        {
            var sb = new StringBuilder();
            var allowed = true;

            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];

                if (allowed && !rule.AllowOptimizations)
                {
                    sb.AppendLine("; #OPTIMIZER END");
                    allowed = false;
                }

                sb.AppendLine($"; {i}");
                sb.AppendLine("(defrule");

                if (rule.Facts.Count == 0)
                {
                    sb.AppendLine("\t(true)");
                }

                foreach (var fact in rule.Facts)
                {
                    sb.AppendLine($"\t{fact.Code}");
                }

                sb.AppendLine("=>");

                if (rule.Actions.Count == 0)
                {
                    sb.AppendLine("\t(do-nothing)");
                }

                foreach (var action in rule.Actions)
                {
                    sb.AppendLine($"\t{action.Code}");
                }

                sb.AppendLine(")");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private Rule ParseRule(string code)
        {
            var rule = new Rule();
            var pieces = code.Split("=>");

            ParseCommands(pieces[0].Split('\n'), rule.Facts);
            ParseCommands(pieces[1].Split('\n'), rule.Actions);

            return rule;
        }

        private void ParseCommands(IEnumerable<string> lines, List<Command> output)
        {
            foreach (var iline in lines)
            {
                var line = iline;
                var cpos = line.IndexOf(";");

                if (cpos >= 0)
                {
                    line = line[..cpos];
                }

                line = line.Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (!line.Contains('('))
                {
                    continue;
                }
                else if (line.Contains("(defconst"))
                {
                    continue;
                }

                output.Add(new() { Code = line });
            }
        }
    }
}
