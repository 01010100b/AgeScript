using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer.Optimizations
{
    public class PushDownToActions : IOptimization
    {
        public int Priority => 0;

        private int MaxElements { get; set; }

        public void Optimize(List<Rule> rules)
        {
            MaxElements = rules.Max(x => x.Elements);

            for (int pass = 0; pass < 3; pass++)
            {
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
                    else if (!current.IsAlwaysTrue || !next.IsAlwaysTrue)
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

                next.Actions.Insert(0, current.Actions[^1]);
                current.Actions.RemoveAt(current.Actions.Count - 1);
            }
        }
    }
}
