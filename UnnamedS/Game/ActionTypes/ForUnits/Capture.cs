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
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private Capture() : base("capture") { }
        public static Capture Instance { get; } = new Capture();

        private const int ACTIONS_NEEDED = 1;

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (sourceTile.Location != targetTile.Location)
                return false;

            if (targetTile.Terrain.TerrainType.CanCapture == false)
                return false;

            if (targetTile.Terrain.IsOwned == true && targetTile.Terrain.CommanderID == sourceTile.Unit.CommanderID)
                return false;

            if (sourceTile.Unit.Actions < ACTIONS_NEEDED)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
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

            if(ourCaptureProgress >= targetTile.Terrain.TerrainType.MaxCapturePoints)
            {
                changeList.Add("IsOwned", true);
                changeList.Add("CommanderID", ourCommanderID);
                ourCaptureProgress = targetTile.Terrain.TerrainType.MaxCapturePoints;
            }

            captureProgress[ourCommanderID] = ourCaptureProgress;

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

            return new List<StateChange>()
            {
                new StateChanges.TerrainStateChange(targetTile.Location, changeList),
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "Actions", sourceTile.Unit.Actions - ACTIONS_NEEDED }
                }, sourceTile.Location)
            };
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
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
                    chain.AddAction(new ActionChain.Link(action, new UnitContext(sourceTile.Location), new TileContext(location)));

                chain.AddAction(new ActionChain.Link(this, new UnitContext(location), new TerrainContext(location)));

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
