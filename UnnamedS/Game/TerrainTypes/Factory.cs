using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Factory : LandStructure
    {
        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.BuildLandUnit.Instance
            };

        private Factory() : base("factory") { }
        public static Factory Instance { get; } = new Factory();
    }
}
