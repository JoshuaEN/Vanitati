using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class DestroyedFactory : DestroyedLandStructure
    {
        private DestroyedFactory() : base("destroyed_factory") { }
        public static DestroyedFactory Instance { get; } = new DestroyedFactory();
    }
}
