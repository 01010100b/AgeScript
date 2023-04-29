using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public class Rule
    {
        public List<Command> Facts { get; } = new();
        public List<Command> Actions { get; } = new();
        public List<string> JumpTargets { get; } = new();
        public bool AllowOptimizations { get; internal set; } = false;
        public IEnumerable<Command> Commands => Facts.Concat(Actions);
        public int Elements => Commands.Sum(x => x.Elements);
        public bool IsJump => Actions.Count > 0 && Actions[^1].IsJump;
        public bool AlwaysTrue => Facts.Count == 1 && Facts[0].Code == "(true)";
    }
}
