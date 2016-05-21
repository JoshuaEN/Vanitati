using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public abstract class SourceContext : Context
    {
        public sealed override bool CanBeSource { get; } = true;
        public sealed override bool CanBeTarget { get; } = false;
    }
}
