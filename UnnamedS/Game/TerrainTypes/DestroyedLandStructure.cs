using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public abstract class DestroyedLandStructure : Land
    {
        protected DestroyedLandStructure(string key) : base(key) { }

        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Rough;
        public override TerrainHeight Height { get; } = TerrainHeight.Normal;
    }
}
