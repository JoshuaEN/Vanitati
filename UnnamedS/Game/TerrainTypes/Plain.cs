using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Plain : TerrainType
    {
        private Plain() : base("plain", new Dictionary<string, object>()) { }
        public static Plain Instance { get; } = new Plain();
    }
}
