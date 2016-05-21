using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForGame
{
    public class DetectVictoryPointWinConditions : GameTargetOtherAction
    {
        private DetectVictoryPointWinConditions() : base("detect_victory_point_win_conditions") {  }
        public static DetectVictoryPointWinConditions Instance { get; } = new DetectVictoryPointWinConditions();

        public override ActionTriggers Triggers { get; } = ActionTriggers.OnRoundStart;

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, GameTargetOtherContext context)
        {
            if ((state.VictoryPointGapEnabled == false && state.VictoryPointLimitEnabled == false) || state.VictoryPointVictoryAchieved == true || state.VictoryPointsPerPoint == 0)
                return new List<StateChange>(0);

            var list = new List<StateChange>();

            var vpDic = new Dictionary<Commander, int>();

            foreach (var commander in state.Commanders.Values)
            {
                var dic = new Dictionary<string, object>();
                var ownedCitiesCount = 0;
                var ownedVictoryPointsCount = 0;

                foreach (var terrain in state.Terrain)
                {
                    if (terrain.IsOwned == false || terrain.CommanderID != commander.CommanderID)
                        continue;

                    if (terrain.TerrainType == TerrainTypes.City.Instance)
                        ownedCitiesCount += 1;
                    else if (terrain.TerrainType.IsVictoryPoint == true)
                        ownedVictoryPointsCount += 1;
                }

                var vp = commander.VictoryPoints;

                if (ownedVictoryPointsCount > 0)
                {
                    vp = commander.VictoryPoints + (ownedVictoryPointsCount * state.VictoryPointsPerPoint);
                    dic.Add("VictoryPoints", vp);
                }

                vpDic.Add(commander, vp);

                list.Add(new StateChanges.CommanderStateChange(commander.CommanderID, dic));
            }


            int vpMax = 0;
            int vp2nd = 0;
            int vpPointGoal = 0;
            if (state.VictoryPointGapEnabled)
            {
                foreach (var kp in vpDic)
                {
                    var vp = kp.Value;
                    if (vp > vpMax)
                    {
                        vp2nd = vpMax;
                        vpMax = vp;
                    }
                    else if (vp > vp2nd)
                    {
                        vp2nd = vp;
                    }
                }

                vpPointGoal = vp2nd + state.VictoryPointGap;
            }

            if (state.VictoryPointLimitEnabled)
            {
                if (state.VictoryPointGapEnabled)
                {
                    if (state.VictoryPointLimit < vpPointGoal)
                        vpPointGoal = state.VictoryPointLimit;
                }
                else
                {
                    vpPointGoal = state.VictoryPointLimit;
                }
            }

            if (vpMax >= vpPointGoal)
            {
                var victors = new List<int>();

                foreach (var kp in vpDic)
                {
                    if (kp.Value > vpPointGoal)
                    {
                        if(kp.Value == vpMax)
                        {
                            victors.Add(kp.Key.CommanderID);
                        }
                    }
                }

                list.Add(new StateChanges.VictoryConditionAchieved(victors.ToArray(), "game_victory_condition_victory_points"));
                list.Add(new StateChanges.GameStateChange(new Dictionary<string, object>() { { "VictoryPointVictoryAchieved", true } }));
            }

            return list;
        }
    }
}
