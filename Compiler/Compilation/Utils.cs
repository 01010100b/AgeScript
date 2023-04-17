using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    internal static class Utils
    {
        public static void MemCopy(RuleList rules, int from, int to, int length, bool ref_from, bool ref_to)
        {
            rules.AddAction($"set-goal sp0 {length}");

            if (ref_from)
            {
                rules.AddAction($"up-get-indirect-goal g: {from} sp1");
            }
            else
            {
                rules.AddAction($"set-goal sp1 {from}");
            }

            if (ref_to)
            {
                rules.AddAction($"up-get-indirect-goal g: {to} sp2");
            }
            else
            {
                rules.AddAction($"set-goal sp2 {to}");
            }

            rules.StartNewRule("up-compare-goal sp0 c:> 0");
            rules.AddAction("up-get-indirect-goal g: sp1 sp3");
            rules.AddAction("up-set-indirect-goal g: sp2 g: sp3");
            rules.AddAction("up-modify-goal sp1 c:+ 1");
            rules.AddAction("up-modify-goal sp2 c:+ 1");
            rules.AddAction("up-modify-goal sp0 c:- 1");
            rules.AddAction("up-jump-rule -1");
            rules.StartNewRule();
        }
    }
}
