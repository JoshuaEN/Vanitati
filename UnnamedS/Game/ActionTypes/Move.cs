using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    public sealed class Move : ActionType, ICausesMovement
    {
        public Move() : base("move", ActionTarget.Empty, causesMovement: true) { }
        public static Move Instance { get; } = new Move();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile, Tile targetTile)
        {
            if (base.CanPerformOn(state, context, sourceTile, targetTile) == false)
                return false;

            var costs = GetRemainingMovement(state, context, sourceTile);

            if(costs.ContainsKey(targetTile.Location) == false)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile, Tile targetTile)
        {
            var changes = new List<StateChange>(1);

            changes.Add(
                new StateChanges.UnitStateChange(
                    sourceTile.Unit.UnitID,
                    new Dictionary<string, object>()
                    {
                        {"Location", targetTile.Terrain.Location },
                        {"Movement", GetRemainingMovement(state, context, sourceTile)[targetTile.Location].RemainingMovement }
                    },
                    targetTile.Location,
                    previousLocation: sourceTile.Location
                )
            );

            return changes;
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile)
        {
            return GetRemainingMovement(state, context, sourceTile).Keys.
                        ToDictionary(loc => loc, loc => new ActionChain(new ActionChain.Link(this, sourceTile.Location, loc)));
        }

        public IReadOnlyDictionary<Location, MovementRemaining> GetRemainingMovement(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile)
        {
            var dic = new Dictionary<Location, MovementRemaining>();
            var evalStack = new Stack<Location>();

            if (sourceTile.Unit == null || sourceTile.Unit.Movement <= 0)
                return dic;

            dic.Add(sourceTile.Terrain.Location, new MovementRemaining(sourceTile.Unit.Movement, sourceTile.Terrain.Location));
            evalStack.Push(sourceTile.Terrain.Location);

            do
            {
                var sourceLocation = evalStack.Pop();

                var availableMovement = dic[sourceLocation];

                foreach(var location in state.LocationsAroundPoint(sourceLocation, 1))
                {
                    var targetTile = state.GetTile(location);
                    if(base.CanPerformOn(state, context, sourceTile, targetTile))
                    {
                        var movementCost = sourceTile.Unit.UnitType.MovementType.GetMovementCost(targetTile.Terrain.TerrainType);

                        var remainingMovement = availableMovement.RemainingMovement - movementCost;

                        if(remainingMovement < 0)
                            continue;

                        MovementRemaining remaining;

                        if (dic.TryGetValue(location, out remaining) == false)
                        {
                            remaining = new MovementRemaining(remainingMovement, location);
                            dic.Add(location, remaining);
                            evalStack.Push(location);
                        }
                        else
                        {
                            if (remaining.UpdateFromLocation(remainingMovement, location) && remainingMovement > 0)
                            {
                                evalStack.Push(location);
                            }
                        }
                    }
                }
            }
            while (evalStack.Count > 0);

            dic.Remove(sourceTile.Location);

            return dic;
        }

        public sealed class MovementRemaining
        {
            public int RemainingMovement { get; private set; }
            public Location FromLocation { get; private set; }

            public MovementRemaining(int remainingMovement, Location fromLocation)
            {
                Contract.Requires<ArgumentNullException>(null != fromLocation);
                Contract.Requires<ArgumentException>(remainingMovement >= 0);

                RemainingMovement = remainingMovement;
                FromLocation = fromLocation;
            }

            public bool UpdateFromLocation(int remainingMovement, Location fromLocation)
            {
                if(remainingMovement > RemainingMovement)
                {
                    RemainingMovement = remainingMovement;
                    FromLocation = FromLocation;

                    return true;
                }
                return false;
            }

            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(null != FromLocation);
                Contract.Invariant(RemainingMovement >= 0);
            }
        }
    }
}
