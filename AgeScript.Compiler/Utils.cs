﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Compiler
{
    internal static class Utils
    {
        public static void Clear(RuleList rules, int from, int length, int value = 0)
        {
            if (length <= 0)
            {
                return;
            }

            while (length > 0)
            {
                if (length >= 4 && from >= 41 && from < 508 && value == 0)
                {
                    rules.AddAction($"up-reset-cost-data {from}");
                    from += 4;
                    length -= 4;
                }
                else
                {
                    rules.AddAction($"up-modify-goal {from} c:= {value}");
                    from++;
                    length--;
                }
                
            }
        }

        public static void MemCopy(CompilationResult result, int from, int to, int length,
            bool ref_from = false, bool ref_to = false,
            int from_offset = 0, int to_offset = 0, bool ref_from_offset = false, bool ref_to_offset = false)
        {
            if (length <= 0)
            {
                return;
            }

            if (result.Settings.OptimizeMemCopy && length < result.Settings.MaxElementsPerRule)
            {
                if (!ref_from && !ref_to && !ref_from_offset && !ref_to_offset)
                {
                    for (int i = 0; i < length; i++)
                    {
                        result.Rules.AddAction($"up-modify-goal {to + to_offset + i} g:= {from + from_offset + i}");
                    }

                    return;
                }
            }

            // set pointers

            if (ref_from)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} g:= {from}");
            }
            else
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} c:= {from}");
            }

            if (from_offset != 0)
            {
                if (ref_from_offset)
                {
                    result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} g:+ {from_offset}");
                }
                else
                {
                    result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} c:+ {from_offset}");
                }
            }

            if (ref_to)
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} g:= {to}");
            }
            else
            {
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} c:= {to}");
            }

            if (to_offset != 0)
            {
                if (ref_to_offset)
                {
                    result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} g:+ {to_offset}");
                }
                else
                {
                    result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} c:+ {to_offset}");
                }
            }

            // loop

            if (result.Settings.OptimizeMemCopy && length <= result.Settings.MaxElementsPerRule / 3)
            {
                // unroll if small length

                for (int i = 0; i < length; i++)
                {
                    result.Rules.AddAction($"up-get-indirect-goal g: {result.Memory.Sp1} {result.Memory.Sp3}");
                    result.Rules.AddAction($"up-set-indirect-goal g: {result.Memory.Sp2} g: {result.Memory.Sp3}");

                    if (i < length - 1)
                    {
                        result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} c:+ 1");
                        result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} c:+ 1");
                    }
                }

                return;
            }

            result.Rules.AddAction($"up-modify-goal {result.Memory.Sp0} c:= {length}");

            if (result.Settings.InlineMemCopy)
            {
                result.Rules.StartNewRule($"up-compare-goal {result.Memory.Sp0} c:> 0");
                var target_loop = result.Rules.CreateJumpTarget();
                result.Rules.ResolveJumpTarget(target_loop);
                result.Rules.AddAction($"up-get-indirect-goal g: {result.Memory.Sp1} {result.Memory.Sp3}");
                result.Rules.AddAction($"up-set-indirect-goal g: {result.Memory.Sp2} g: {result.Memory.Sp3}");
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} c:+ 1");
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} c:+ 1");
                result.Rules.AddAction($"up-modify-goal {result.Memory.Sp0} c:- 1");
                result.Rules.AddAction($"up-jump-direct c: {target_loop}");
                result.Rules.StartNewRule();
            }
            else
            {
                var target = result.Rules.CreateJumpTarget();
                result.Rules.AddAction($"up-modify-goal {result.Memory.NonInlinedMemCopyReturnAddr} c:= {target}");
                result.Rules.AddAction($"up-jump-direct c: {result.Rules.MemCopyTarget}");
                result.Rules.StartNewRule();
                result.Rules.ResolveJumpTarget(target);
            }
        }

        public static void CompileMemCopy(CompilationResult result)
        {
            if (result.Settings.InlineMemCopy)
            {
                return;
            }

            result.Rules.StartNewRule($"up-compare-goal {result.Memory.Sp0} c:> 0");
            result.Rules.ResolveJumpTarget(result.Rules.MemCopyTarget);
            var target_loop = result.Rules.CreateJumpTarget();
            result.Rules.ResolveJumpTarget(target_loop);
            result.Rules.AddAction($"up-get-indirect-goal g: {result.Memory.Sp1} {result.Memory.Sp3}");
            result.Rules.AddAction($"up-set-indirect-goal g: {result.Memory.Sp2} g: {result.Memory.Sp3}");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Sp1} c:+ 1");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Sp2} c:+ 1");
            result.Rules.AddAction($"up-modify-goal {result.Memory.Sp0} c:- 1");
            result.Rules.AddAction($"up-jump-direct c: {target_loop}");

            result.Rules.StartNewRule();
            result.Rules.AddAction($"up-jump-direct g: {result.Memory.NonInlinedMemCopyReturnAddr}");
            result.Rules.StartNewRule();
        }
    }
}
