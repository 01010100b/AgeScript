using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeScript.Optimizer
{
    public class Command
    {
        private static readonly HashSet<string> JumpCommands = new() { "up-jump-direct", "up-jump-rule", "up-jump-dynamic" };
        
        public string Code { get; set; } = string.Empty;
        public int Elements => Code.Count(x => x == '(');
        public bool IsCompound => Elements > 1;
        public string Id => IsCompound ? throw new Exception("Can not get id of compound command") : GetId();
        public bool IsJump => JumpCommands.Contains(Id);

        private string GetId()
        {
            var id = Code.Split(' ')[0].Replace("(", string.Empty).Replace(")", string.Empty).Trim();

            return id;
        }
    }
}
