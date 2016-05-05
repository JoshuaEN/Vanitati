using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class Move : UnitTargetTileAction, ICausesMovement
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        public override bool CausesMovement
        {
            get { return true; }
        }

        private Move() : base("move") { }
        public static Move Instance { get; } = new Move();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (TileChecks(sourceTile, targetTile) == false)
                return false;

            var costs = GetRemainingMovement(state, context, sourceTile);

            if(costs.ContainsKey(targetTile.Location) == false)
                return false;

            return true;
        }

        private bool TileChecks(Tile sourceTile, Tile targetTile)
        {
            if (sourceTile.Unit == null)
                return false;

            if (targetTile.Unit != null)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var remainingMovement = GetRemainingMovement(state, context, sourceTile)[targetTile.Location].RemainingMovement;
            var dic = new Dictionary<string, object>()
            {
                {"Location", targetTile.Terrain.Location },
                {"Movement",  remainingMovement }
            };

            var movementSpent = sourceTile.Unit.Movement - remainingMovement;
            var supplies = sourceTile.Unit.Supplies.ToDictionary(kp => kp.Key, kp => kp.Value);

            foreach(var kp in sourceTile.Unit.UnitType.MovementSupplyUsage)
            {
                supplies[kp.Key] -= kp.Value * movementSpent;
            }

            dic.Add("Supplies", supplies);

            var changes = new List<StateChange>(1);

            changes.Add(
                new StateChanges.UnitStateChange(
                    sourceTile.Unit.UnitID,
                    dic,
                    targetTile.Location,
                    previousLocation: sourceTile.Location
                )
            );

            return changes;
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            return GetRemainingMovement(state, context, sourceTile).Keys.
                        ToDictionary(loc => loc, loc => new ActionChain(new ActionChain.Link(this, new UnitContext(sourceTile.Location), new TerrainContext(loc))));
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var remainingMovement = GetRemainingMovement(state, context, sourceTile);

            var typeVisitCount = new Dictionary<TerrainType, int>();

            var location = targetTile.Location;

            while(location != sourceTile.Location)
            {
                var terrain = state.GetTerrain(location);
                var movement = remainingMovement[location];

                int times;

                if (typeVisitCount.TryGetValue(terrain.TerrainType, out times) == false)
                    times = 0;
                times += 1;

                typeVisitCount[terrain.TerrainType] = times;

                location = movement.FromLocation;
            }

            var typeOrder = typeVisitCount.OrderByDescending(kp => sourceTile.Unit.UnitType.MovementType.GetMovementCost(kp.Key));

            var list = new List<Modifier>();

            foreach(var kp in typeOrder)
            {
                var cost = sourceTile.Unit.UnitType.MovementType.GetMovementCost(kp.Key);
                list.Add(
                    new ModifierForumla("movement_cost_for", cost * kp.Value,
                        new Modifier(kp.Key.Key, null),
                        new Modifier("movement_cost", cost),
                        ModifierForumla.OPERATOR_MULTIPLY,
                        new Modifier("times_traversed", kp.Value)
                    )
               );
            }

            return new List<Modifier>()
            {
                new ModifierForumla("overall_movement_cost", sourceTile.Unit.Movement - remainingMovement[targetTile.Location].RemainingMovement, list.ToArray())
            };
        }

        public IReadOnlyDictionary<Location, MovementRemaining> GetRemainingMovement(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            var dic = new Dictionary<Location, MovementRemaining>();

            if (sourceTile.Unit == null || sourceTile.Unit.Movement <= 0)
                return dic;

            var evalStack = new Stack<Location>();

            var maxMovementAvailable = sourceTile.Unit.Movement;

            foreach(var kp in sourceTile.Unit.UnitType.MovementSupplyUsage)
            {
                var currentSupply = sourceTile.Unit.Supplies[kp.Key];

                var maxMovementGivenSupply = currentSupply / kp.Value;

                if (maxMovementGivenSupply < maxMovementAvailable)
                    maxMovementAvailable = maxMovementGivenSupply;
            }

            // Initialize with the current tile as a starting point.
            dic.Add(sourceTile.Terrain.Location, new MovementRemaining(maxMovementAvailable, sourceTile.Terrain.Location));
            evalStack.Push(sourceTile.Terrain.Location);

            do
            {
                var sourceLocation = evalStack.Pop();

                // Get the available movement from the current location.
                var availableMovement = dic[sourceLocation];

                // For each of the tiles around the current location,
                foreach(var location in state.LocationsAroundPoint(sourceLocation, 1))
                {
                    var targetTile = state.GetTile(location);

                    if (targetTile == null)
                        continue;

                    // Check that the location can be traversed.
                    if(TileChecks(sourceTile, targetTile))
                    {
                        var movementCost = sourceTile.Unit.UnitType.MovementType.GetMovementCost(targetTile.Terrain.TerrainType);

                        var remainingMovement = availableMovement.RemainingMovement - movementCost;

                        // If there is less than 0 movement remaining, the unit can't move to this tile.
                        if(remainingMovement < 0)
                            continue;

                        MovementRemaining remaining;

                        if (dic.TryGetValue(location, out remaining) == false)
                        {
                            remaining = new MovementRemaining(remainingMovement, sourceLocation);
                            dic.Add(location, remaining);

                            if(remainingMovement > 0)
                                evalStack.Push(location);
                        }
                        else
                        {
                            if (remaining.UpdateFromLocation(remainingMovement, sourceLocation) && remainingMovement > 0)
                            {
                                evalStack.Push(location);
                            }
                        }
                    }
                }
            }
            while (evalStack.Count > 0);

            // Remove the starting tile because we can't move to the tile we started on.
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
                Contract.Requires<ArgumentNullException>(null != fromLocation);
                Contract.Requires<ArgumentException>(remainingMovement >= 0);

                if (remainingMovement > RemainingMovement)
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
