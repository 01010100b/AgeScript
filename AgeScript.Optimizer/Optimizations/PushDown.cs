using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AgeScript.Optimizer.Optimizations
{
    public class PushDown : IOptimization
    {
        private static readonly List<string> PushDowns = new() { "up-modify-goal" };

        private int MaxElements { get; set; }

        public void Optimize(List<Rule> rules)
        {
            MaxElements = rules.Max(x => x.Elements);

            for (int i = 0; i < rules.Count - 1; i++)
            {
                var current = rules[i];
                var next = rules[i + 1];

                if (!current.AllowOptimizations || !next.AllowOptimizations)
                {
                    continue;
                }
                else if (next.JumpTargets.Count > 0)
                {
                    continue;
                }
                else if (!current.AlwaysTrue)
                {
                    continue;
                }
                else if (current.IsJump)
                {
                    continue;
                }

                Optimize(current, next);
            }

            Utils.RemoveEmptyRules(rules);
        }

        private void Optimize(Rule current, Rule next)
        {
            while (true)
            {
                if (current.Actions.Count == 0)
                {
                    break;
                }
                else if (next.Elements >= MaxElements)
                {
                    break;
                }

                var action = current.Actions[^1];

                if (action.IsCompound)
                {
                    break;
                }

                var can = false;

                foreach (var pushdown in PushDowns)
                {
                    if (action.Code.Contains(pushdown))
                    {
                        can = true;

                        break;
                    }
                }

                if (!can)
                {
                    break;
                }

                if (next.Facts.Count == 1 && next.Facts[0].Code == "(true)")
                {
                    next.Facts.RemoveAt(0);
                }

                next.Facts.Insert(0, current.Actions[^1]);
                current.Actions.RemoveAt(current.Actions.Count - 1);
            }
        }
    }
}
