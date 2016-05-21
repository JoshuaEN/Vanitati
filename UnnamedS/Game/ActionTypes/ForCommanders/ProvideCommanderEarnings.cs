using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    public sealed class ProvideCommanderEarnings : CommanderTargetOtherAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnTurnStart;

        public ProvideCommanderEarnings() : base("provide_commander_earnings") { }
        public static ProvideCommanderEarnings Instance { get; } = new ProvideCommanderEarnings();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {

            var dic = new Dictionary<string, object>();

            var ownedCitiesCount = 0;

            foreach (var terrain in state.Terrain)
            {
                if (terrain.IsOwned == false || terrain.CommanderID != sourceCommander.CommanderID)
                    continue;

                if (terrain.TerrainType == TerrainTypes.City.Instance)
                    ownedCitiesCount += 1;
            }

            if (ownedCitiesCount > 0 && state.CreditsPerCity != 0)
            {
                dic.Add("Credits", sourceCommander.Credits + (ownedCitiesCount * state.CreditsPerCity));
            }


            if (dic.Count > 0)
            {
                return new List<StateChange>()
                {
                    new StateChanges.CommanderStateChange(sourceCommander.CommanderID, dic)
                };
            }
            else
            {
                return new List<StateChange>(0);
            }
        }

        private int GetOwnedCities(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            return state.Terrain.Where(t => t.TerrainType == TerrainTypes.City.Instance && t.IsOwned && t.CommanderID == sourceCommander.CommanderID).Count();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            return new List<Modifier>()
            {
                new ModifierForumla("per_a_turn_commander_earnings", GetOwnedCities(state, context, sourceCommander) * state.CreditsPerCity,
                    new Modifier("cities_owned", GetOwnedCities(state, context, sourceCommander)),
                    ModifierForumla.OPERATOR_MULTIPLY,
                    new Modifier("credits_per_city", state.CreditsPerCity)
                )
            };
        }
    }
}
