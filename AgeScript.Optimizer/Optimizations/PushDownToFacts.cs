﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AgeScript.Optimizer.Optimizations
{
    public class PushDownToFacts : IOptimization
    {
        private static readonly List<string> PushDowns = new() { "up-modify-goal", "up-get-fact", "up-get-focus-fact" };

        public void Optimize(List<Rule> rules)
        {
            var max_elements = rules.Max(x => x.Elements);

            for (int i = 0; i < rules.Count - 1; i++)
            {
                var current = rules[i];
                var next = rules[i + 1];

                if (!current.AllowsOptimizations || !next.AllowsOptimizations)
                {
                    continue;
                }
                else if (next.JumpTargets.Count > 0)
                {
                    continue;
                }
                else if (!current.IsAlwaysTrue)
                {
                    continue;
                }
                else if (current.IsJump)
                {
                    continue;
                }

                Optimize(current, next, max_elements);
            }

            Utils.RemoveEmptyRules(rules);
        }

        private void Optimize(Rule current, Rule next, int max_elements)
        {
            while (true)
            {
                if (current.Actions.Count == 0)
                {
                    break;
                }
                else if (next.Elements >= max_elements)
                {
                    break;
                }

                var action = current.Actions[^1];

                if (action.IsCompound)
                {
                    break;
                }

                var can_push = false;

                foreach (var pushdown in PushDowns)
                {
                    if (action.Code.Contains(pushdown))
                    {
                        can_push = true;

                        break;
                    }
                }

                if (!can_push)
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
