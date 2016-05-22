using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class Capture : UnitTargetTileAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser | ActionTriggers.DirectlyByGameLogic;
        public override RepeatFlags RepeatOn { get; } = RepeatFlags.OnTurnEnd;

        private Capture() : base("capture") { }
        public static Capture Instance { get; } = new Capture();

        private const int ACTIONS_NEEDED = 1;

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (sourceTile.Location != targetTile.Location)
                return false;

            if (targetTile.Terrain.TerrainType.CanCapture == false)
                return false;

            if (targetTile.Terrain.IsOwned == true && targetTile.Terrain.CommanderID == sourceTile.Unit.CommanderID && targetTile.Terrain.CaptureProgress.Count == 0)
                return false;

            if (sourceTile.Unit.Actions < ACTIONS_NEEDED)
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

            var list = new List<StateChange>();
            var changeList = new Dictionary<string, object>();

            var captureProgress = targetTile.Terrain.CaptureProgress.ToDictionary(kp => kp.Key, kp => kp.Value);
            var ourCommanderID = sourceTile.Unit.CommanderID;
            var ourCaptureStrength = GetCaptureStrength(state, context, sourceTile, targetTile);

            int ourCaptureProgress;

            if(captureProgress.TryGetValue(ourCommanderID, out ourCaptureProgress) == false)
            {
                ourCaptureProgress = 0;
                captureProgress.Add(ourCommanderID, ourCaptureProgress);
            }

            ourCaptureProgress += ourCaptureStrength;


            if (targetTile.Terrain.IsOwned && captureProgress.ContainsKey(targetTile.Terrain.CommanderID) == false)
            {
                if(targetTile.Terrain.TerrainType.MaxCapturePoints > ourCaptureStrength)
                {
                    captureProgress[targetTile.Terrain.CommanderID] = targetTile.Terrain.TerrainType.MaxCapturePoints;
                }
            }

            if (ourCaptureProgress >= targetTile.Terrain.TerrainType.MaxCapturePoints)
            {
                changeList.Add("IsOwned", true);
                changeList.Add("CommanderID", ourCommanderID);
                captureProgress.Remove(ourCommanderID);
                list.Add(GetClearRepeatedActionChange(sourceTile));
            }
            else
            {
                captureProgress[ourCommanderID] = ourCaptureProgress;
            }

            foreach(var cmdID in captureProgress.Keys.ToList())
            {
                if (cmdID == ourCommanderID)
                    continue;

                var capProgress = captureProgress[cmdID];

                if (capProgress <= ourCaptureStrength)
                    captureProgress.Remove(cmdID);
                else
                    captureProgress[cmdID] = capProgress - ourCaptureStrength;
            }

            

            changeList.Add("CaptureProgress", captureProgress);


            list.Add(new StateChanges.TerrainStateChange(targetTile.Location, changeList));
            list.Add(new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "Actions", sourceTile.Unit.Actions - ACTIONS_NEEDED }
                }, sourceTile.Location)
            );

            return list;
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
                new ModifierForumla( "unit_capture_strength", GetCaptureStrength(state, context, sourceTile, targetTile),
                    new Modifier("unit_health", sourceTile.Unit.Health) 
                )
            };
        }

        private int GetCaptureStrength(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return sourceTile.Unit.Health;
        }
    }
}
