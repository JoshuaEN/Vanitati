using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    public class RepairAndResupply : TerrainTargetOtherAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnOccupyingUnitTurnStart;

        private RepairAndResupply() : base("repair_and_resupply") { }
        public static RepairAndResupply Instance { get; } = new RepairAndResupply();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            if (sourceTile.Unit == null)
                return new List<StateChange>();

            if (sourceTile.Terrain.TerrainType.CanCapture)
            {
                if (sourceTile.Terrain.IsOwned == false)
                    return new List<StateChange>();

                if (sourceTile.Unit.CommanderID != sourceTile.Terrain.CommanderID)
                    return new List<StateChange>();
            }

            var changes = new Dictionary<string, object>();
            if (sourceTile.Terrain.TerrainType.CanSupply)
            {

                

                var supplies = sourceTile.Unit.Supplies.ToDictionary(kp => kp.Key, kp => kp.Value);

                var suppliesChanged = false;
                foreach (var supply in sourceTile.Unit.Supplies)
                {
                    var maxSupply = sourceTile.Unit.UnitType.SupplyLimits[supply.Key];

                    if (maxSupply > supply.Value)
                    {
                        int suppliesGained;

                        if (sourceTile.Terrain.TerrainType.ResuppliesPerTurn.TryGetValue(supply.Key, out suppliesGained))
                        {
                            if (supply.Value + suppliesGained > maxSupply)
                                suppliesGained = maxSupply;
                            else
                                suppliesGained += supply.Value;

                            supplies[supply.Key] = suppliesGained;

                            suppliesChanged = true;
                        }
                    }
                }

                if (suppliesChanged)
                {
                    changes["Supplies"] = supplies;
                }
            }

            if(sourceTile.Terrain.TerrainType.CanRepair && sourceTile.Unit.Health < sourceTile.Unit.UnitType.MaxHealth)
            {
                int healthRepaired;

                if(sourceTile.Terrain.TerrainType.RepairsPerTurn.TryGetValue(sourceTile.Unit.UnitType.MovementType, out healthRepaired))
                {
                    var currentHealth = sourceTile.Unit.Health;

                    currentHealth += healthRepaired;

                    if (currentHealth > sourceTile.Unit.UnitType.MaxHealth)
                        currentHealth = sourceTile.Unit.UnitType.MaxHealth;

                    changes["Health"] = currentHealth;
                }
            }

            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, changes, sourceTile.Location)
            };

        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            throw new NotSupportedException();
        }
    }
}
