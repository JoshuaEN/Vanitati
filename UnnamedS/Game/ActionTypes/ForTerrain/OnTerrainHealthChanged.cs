using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    public class OnTerrainHealthChanged : TerrainTargetGenericAction<IDictionary<string, object>>
    {
        private OnTerrainHealthChanged() : base("on_terrain_health_changed") { }
        public static OnTerrainHealthChanged Instance { get; } = new OnTerrainHealthChanged();

        public override ActionTriggers Triggers { get; } = ActionTriggers.OnPropertyChanged;

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<IDictionary<string, object>> context, Tile sourceTile, IDictionary<string, object> targetValue)
        {
            return true;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetGenericContext<IDictionary<string, object>> context, Tile sourceTile, IDictionary<string, object> targetValue)
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<IDictionary<string, object>> context, Tile sourceTile, IDictionary<string, object> targetValue)
        {
            if (sourceTile.Terrain.TerrainType.CanBePillage == false || sourceTile.Terrain.Health > 0 || targetValue.ContainsKey("Health") == false)
                return new List<StateChange>(0);

            return new List<StateChange>()
            {
                new StateChanges.TerrainStateChange(sourceTile.Location, new Dictionary<string, object>()
                {
                    { "TerrainType", sourceTile.Terrain.TerrainType.BecomesWhenDestroyed },
                    { "Health", sourceTile.Terrain.TerrainType.BecomesWhenDestroyed.MaxHealth },
                    { "IsOwned", sourceTile.Terrain.IsOwned && sourceTile.Terrain.TerrainType.BecomesWhenDestroyed.CanCapture },
                    { "CommanderID", sourceTile.Terrain.CommanderID },
                    { "DigIn", 0 }
                })
            };
        }

        public override IReadOnlyList<IDictionary<string, object>> ValidTargets(IReadOnlyBattleGameState state, TerrainTargetGenericContext<IDictionary<string, object>> context, Tile sourceTile)
        {
            throw new NotImplementedException();
        }
    }
}
