using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public class Embark : UnitTargetTileAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private Embark() : base("embark") { }
        public static Embark Instance { get; } = new Embark();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (sourceTile.Unit.Actions <= 0)
                return false;

            if (targetTile.Unit == null)
                return false;

            if (state.IsCommanderFriendly(sourceTile.Unit.CommanderID, targetTile.Unit.CommanderID) == false)
                return false;

            if (targetTile.Unit.EmbarkedUnits.Count >= targetTile.Unit.UnitType.MaxEmbarkedUnits)
                return false;

            if (targetTile.Unit.UnitType.EmbarkableMovementTypes.Contains(sourceTile.Unit.UnitType.MovementType) == false)
                return false;

            if (state.LocationsAroundPoint(targetTile.Location, 1, 1).Contains(sourceTile.Location) == false)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var newEmbarkedUnits = targetTile.Unit.EmbarkedUnits.ToList();
            newEmbarkedUnits.Add(sourceTile.Unit);

            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "Actions", sourceTile.Unit.Actions - 1 },
                    { "Embarked", true }
                }, sourceTile.Location, StateChanges.UnitStateChange.Cause.Removed),
                new StateChanges.UnitStateChange(targetTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "EmbarkedUnits", newEmbarkedUnits }
                }, targetTile.Unit.Location)
            };
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return new List<Modifier>(0);
        }

        protected override bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return CanPerformOn(state, context, sourceTile, targetTile);
        }

        public override IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            return RangeBasedValidTargets(state, context, sourceTile, 1, 1);
        }
    }
}
