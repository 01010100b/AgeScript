using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public static class Utils
    {
        public static void RemoveEmptyRules(List<Rule> rules)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var current = rules[i];

                if (current.Actions.Count > 0)
                {
                    continue;
                }

                if (i == rules.Count - 1)
                {
                    if (current.JumpTargets.Count == 0)
                    {
                        rules.RemoveAt(i);
                    }

                    break;
                }

                var next = rules[i + 1];

                foreach (var target in current.JumpTargets)
                {
                    next.JumpTargets.Add(target);
                }

                rules.RemoveAt(i);
                i--;
            }
        }
    }
}
