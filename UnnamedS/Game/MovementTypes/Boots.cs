using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.MovementTypes
{
    public sealed class Boots : MovementType
    {
        private Boots() : base("boots") { }
        public static Boots Instance { get; } = new Boots();
    }
}
