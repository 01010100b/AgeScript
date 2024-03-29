﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public class ScriptOptimizer
    {
        private const int PASSES = 3;

        public void Optimize(ref string jtp, ref Dictionary<string, int> jump_targets)
        {
            Console.WriteLine("Optimizing...");

            var parser = new PerParser();
            var rules = parser.Parse(jtp);

            foreach (var target in jump_targets)
            {
                if (target.Value >= rules.Count)
                {
                    continue;
                }

                rules[target.Value].JumpTargets.Add(target.Key);
            }

            WriteState(rules);

            Utils.RemoveEmptyRules(rules);

            for (int pass = 0; pass < PASSES; pass++)
            {
                foreach (var optimization in GetOptimizations())
                {
                    optimization.Optimize(rules);
                }
            }

            for (int i = 0; i < rules.Count; i++)
            {
                foreach (var target in rules[i].JumpTargets)
                {
                    jump_targets[target] = i;
                }
            }

            jtp = parser.Write(rules);

            WriteState(rules);
        }

        private void WriteState(IReadOnlyList<Rule> rules)
        {
            Console.WriteLine($"Rules: {rules.Count}");
            Console.WriteLine($"Allowed rules: {rules.Count(x => x.AllowsOptimizations)}");
            Console.WriteLine($"Rules which jump: {rules.Count(x => x.IsJump)}");
            Console.WriteLine($"Always true rules: {rules.Count(x => x.IsAlwaysTrue)}");
            Console.WriteLine($"Commands: {rules.Sum(x => x.Elements)} with {rules.Sum(x => x.Elements) / (double)rules.Count:N2} commands per rule.");
            Console.WriteLine($"Compound commands: {rules.Sum(x => x.Commands.Count(x => x.IsCompound))}");
        }

        private IEnumerable<IOptimization> GetOptimizations()
        {
            var optimizations = new List<IOptimization>();
            var assembly = typeof(IOptimization).Assembly;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(IOptimization)) && !type.IsAbstract)
                {
                    var optimization = Activator.CreateInstance(type);

                    if (optimization is not null)
                    {
                        optimizations.Add((IOptimization)optimization);
                    }
                }
            }

            return optimizations;
        }
    }
}
