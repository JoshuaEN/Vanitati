using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game.ActionTypes.ForUnits;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitAction : TileAction
    {
        public abstract ActionTriggers Triggers { get; }

        public virtual bool CausesMovement { get; }

        public sealed override bool CanUserTrigger
        {
            get { return Triggers.HasFlag(ActionTriggers.ManuallyByUser); }
        }

        public sealed override Category ActionCategory
        {
            get { return Category.Unit; }
        }

        protected UnitAction(string key) : base("unit_" + key) { }

        [Flags]
        public enum ActionTriggers {
            ManuallyByUser = 1,
            DirectlyByGameLogic = 2,
            OnTurnStart = 4,
            OnTurnEnd = 8,
            OnUnitCreated = 16,
            OnUnitDestroyed = 32,
            OnPropertyChanged = 64,
            OnActionPerformedByUser = 128,
            AttackRetaliation = 256,
            None = 0
        }

        protected virtual Dictionary<Location, ActionChain> RangeBasedValidTargets(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, int min, int max)
        {
            var remainingMovementTracking = new Dictionary<Location, int>();
            var dic = new Dictionary<Location, ActionChain>();

            if (sourceTile.Unit == null)
                return dic;

            // Get a list of possible tiles we could attack from (via movement).

            var possibleLocations = new Dictionary<Location, Move.MovementRemaining>();

            foreach (var action in sourceTile.Unit.UnitType.Actions.Where(a => a is ICausesMovement).Cast<ICausesMovement>())
            {
                var canidateMovements = action.GetRemainingMovement(state, context, sourceTile);

                foreach (var kp in canidateMovements)
                {
                    Move.MovementRemaining movementRemaining;

                    if (possibleLocations.TryGetValue(kp.Key, out movementRemaining))
                    {
                        throw new NotImplementedException();
                        //movementRemaining.UpdateFromLocation(kp.Value.MovementActionType, kp.Value.RemainingMovement, kp.Value.FromLocation);
                    }
                    else
                    {
                        possibleLocations.Add(kp.Key, kp.Value);
                    }
                }
            }

            // Add the current tile we're on (it isn't returned above because we can't move to the tile we're on).
            // We pass null in as the action because we don't need to move.
            possibleLocations.Add(sourceTile.Location, null);

            foreach (var kp in possibleLocations)
            {
                var possibleLocation = kp.Key;
                var movementRemaining = kp.Value;

                var currentTile = state.GetTile(possibleLocation);
                if (currentTile == null)
                    continue;

                // Check every tile within range of our attack from the location.
                foreach (var location in state.LocationsAroundPoint(possibleLocation, min, max))
                {
                    var targetTile = state.GetTile(location);

                    if (targetTile == null)
                        continue;

                    // Check that we can attack the unit on this location.
                    if (RangeBasedValidTargetCanPerform(state, context, new Tile(currentTile.Terrain, sourceTile.Unit), targetTile))
                    {
                        int remainingMovement;
                        if (remainingMovementTracking.TryGetValue(location, out remainingMovement) == true)
                        {
                            if (movementRemaining != null && movementRemaining.RemainingMovement <= remainingMovement)
                                continue;
                        }

                        // Create the action chain and register it to our final list.
                        var chain = new ActionChain();

                        if (movementRemaining != null)
                        {
                            chain.AddAction(movementRemaining.MovementActionType, new UnitContext(sourceTile.Location), new GenericContext(possibleLocation));
                        }
                        chain.AddAction(this, new UnitContext(possibleLocation), new GenericContext(location));

                        dic[location] = chain;

                        if (movementRemaining != null)
                            remainingMovementTracking[location] = movementRemaining.RemainingMovement;
                        else
                            remainingMovementTracking[location] = sourceTile.Unit.Movement;
                    }
                }
            }

            return dic;
        }

        protected abstract bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile);

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(Triggers != ActionTriggers.OnPropertyChanged || (typeof(ForUnits.UnitTargetGenericAction<IDictionary<string, object>>).IsAssignableFrom(GetType())));
            Contract.Invariant(Triggers != ActionTriggers.OnActionPerformedByUser || (typeof(ForUnits.UnitTargetGenericAction<ActionInfo>).IsAssignableFrom(GetType())));
        }

    }
}
