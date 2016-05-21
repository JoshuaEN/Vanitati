using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    public abstract class BuildUnit : TerrainTargetGenericAction<UnitType>
    {

        public IReadOnlyList<UnitType> BuildableTypes { get; }

        protected BuildUnit(string key, IReadOnlyList<UnitType> buildableTypes) : base(key)
        {
            Contract.Requires<ArgumentNullException>(null != buildableTypes);
            Contract.Requires<ArgumentException>(buildableTypes.Count > 0);
            
            BuildableTypes = buildableTypes;
        }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<UnitType> context, Tile sourceTile, UnitType targetValue)
        {
            var terrain = sourceTile.Terrain;

            if (sourceTile.Unit != null)
                return false;

            if (terrain.IsOwned == false)
                return false;

            var commander = state.GetCommander(sourceTile.Terrain.CommanderID);

            if (commander == null)
                return false;

            if (BuildableTypes.Contains(targetValue) == false)
                return false;

            if (commander.Credits < targetValue.BuildCost)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<UnitType> context, Tile sourceTile, UnitType targetValue)
        {
            var commander = state.GetCommander(sourceTile.Terrain.CommanderID);
            var newUnit = new Unit(state.GetNextUnitID(), targetValue, sourceTile.Location, commander.CommanderID);
            return new List<StateChange>()
            {
                new StateChanges.CommanderStateChange(sourceTile.Terrain.CommanderID, new Dictionary<string, object>()
                {
                    { "Credits",  commander.Credits - targetValue.BuildCost }
                }),
                new StateChanges.UnitStateChange(newUnit.UnitID, newUnit.GetWriteableProperties(), newUnit.Location, StateChanges.UnitStateChange.Cause.Created)
            };
        }

        public override IReadOnlyList<UnitType> ValidTargets(IReadOnlyBattleGameState state, TerrainTargetGenericContext<UnitType> context, Tile sourceTile)
        {
            return BuildableTypes.Where(t => CanPerformOn(state, context, sourceTile, t)).ToList();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetGenericContext<UnitType> context, Tile sourceTile, UnitType targetValue)
        {
            return new List<Modifier>()
            {
                new Modifier("build_cost", targetValue.BuildCost)
            };
        }
    }
}
