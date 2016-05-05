using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public static class Preloader
    {
        public static void Preload()
        {
            object v;

            v = SupplyType.TYPES;
            v = MovementType.TYPES;
            BaseType.BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForUnits");
            v = UnitType.TYPES;
            BaseType.BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForTerrain");
            v = TerrainType.TYPES;
            BaseType.BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForCommanders");
            v = CommanderType.TYPES;

            ActionType.InitActionType();

        }
    }
}
