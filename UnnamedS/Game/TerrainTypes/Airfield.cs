using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Airfield : LandStructure
    {
        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.BuildAirUnit.Instance
            };

        private Airfield() : base("airfield") { }
        public static Airfield Instance { get; } = new Airfield();
    }
}
