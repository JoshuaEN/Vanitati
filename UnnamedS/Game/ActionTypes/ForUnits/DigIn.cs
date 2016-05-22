using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class DigIn : UnitTargetTileAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser | ActionTriggers.DirectlyByGameLogic;

        public override RepeatFlags RepeatOn { get; } = RepeatFlags.OnTurnStart;

        private DigIn() : base("dig_in") { }
        public static DigIn Instance { get; } = new DigIn();

        private const int ACTIONS_REQUIRED = 1;

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (sourceTile.Location != targetTile.Location)
                return false;

            if (targetTile.Terrain.DigIn >= targetTile.Terrain.TerrainType.DigInCap)
                return false;

            if (sourceTile.Unit.Actions < ACTIONS_REQUIRED)
                return false;

            if (context.Trigger == ActionTriggers.ManuallyByUser && sourceTile.Unit.RepeatedAction.Type == Instance)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {

            if (context.Trigger == ActionTriggers.ManuallyByUser)
            {
                return new List<StateChange>()
                {
                    GetRepeatedActionChange(state, sourceTile, new GenericContext(targetTile.Location))
                };
            }
            else
            {
                var list = new List<StateChange>();

                list.Add(new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                    {
                        { "Actions", sourceTile.Unit.Actions - ACTIONS_REQUIRED }
                    }, sourceTile.Location));
                list.Add(new StateChanges.TerrainStateChange(targetTile.Location, new Dictionary<string, object>()
                    {
                        { "DigIn", targetTile.Terrain.DigIn + 1 }
                    }));

                if (targetTile.Terrain.DigIn + 1 >= targetTile.Terrain.TerrainType.DigInCap)
                    list.Add(GetClearRepeatedActionChange(sourceTile));

                return list;
            }
        }

        protected override bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return CanPerformOn(state, context, sourceTile, targetTile);
        }

        public override IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            return RangeBasedValidTargets(state, context, sourceTile, 0, 0);

            var listing = new Dictionary<Location, ActionChain>();

            var movement = sourceTile.Unit.GetAvailableMovement(state, context, sourceTile);
            movement.Add(sourceTile.Location, null);

            foreach (var kp in movement)
            {
                var location = kp.Key;
                var action = kp.Value;
                var targetTile = state.GetTile(location);
                if (CanPerformOn(state, context, new Tile(targetTile.Terrain, sourceTile.Unit), targetTile) == false)
                    continue;

                var chain = new ActionChain();

                if (action != null)
                    chain.AddAction(new ActionChain.Link(action, new UnitContext(sourceTile.Location), new GenericContext(location)));

                chain.AddAction(new ActionChain.Link(this, new UnitContext(location), new GenericContext(location)));

                listing.Add(location, chain);
            }

            return listing;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return new List<Modifier>()
            {
                new Modifier("dig_in_increase", 1),
                new Modifier("action_cost", ACTIONS_REQUIRED)
            };
        }
    }
}
