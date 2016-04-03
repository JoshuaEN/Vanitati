using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Game
{
    public class LocalGameLogic : GameLogic
    {
        protected Dictionary<int, PlayerSet> PlayerList { get; } = new Dictionary<int, PlayerSet>();

        public LocalGameLogic(BattleGameState state) : base(state)
        {

        }

        public override void AddPlayer(IReadOnlyList<IPlayerLogic> logic)
        {
            var id = AddPlayerLocally(logic);
            var eventArgs = new Event.ThisPlayerAddedArgs(id);
            foreach(var _logic in logic)
            {
                _logic.OnThisPlayerAdded(this, eventArgs);
            }

            var changeArgs = new Event.PlayerChangedEventArgs(new PlayerStateChange(id, new List<IAttribute>(), PlayerStateChange.Cause.Added));
            foreach(var l in PlayersToNotify())
            {
                l.OnPlayerChanged(this, changeArgs);
            }
        }

        public int AddPlayerLocally(IReadOnlyList<IPlayerLogic> logic)
        {
            var playerID = State.AddPlayer();
            PlayerList.Add(playerID, new PlayerSet(logic, new Player(playerID)));
            return playerID;
        }

        public override void RemovePlayer(int playerID)
        {
            State.RemovePlayer(playerID);
            PlayerList.Remove(playerID);
            var changeArgs = new Event.PlayerChangedEventArgs(new PlayerStateChange(playerID, new List<IAttribute>(), PlayerStateChange.Cause.Removed));
            foreach(var l in PlayersToNotify())
            {
                l.OnPlayerChanged(this, changeArgs);
            }
        }

        public override void DoAction(int playerID, ActionInfo action)
        {
            var changes = action.Type.PerformOn(
                State, 
                new Action.ActionContext(playerID, ActionType.ActionTriggers.None), 
                State.GetTile(action.Source), State.GetTile(action.Target));

            foreach(var change in changes)
            {
                if (change is GameStateChange)
                {
                    var castedChange = (change as GameStateChange);
                    State.UpdateGame(castedChange);

                    var args = new Event.GameStateChangedArgs(castedChange);
                    foreach (var l in PlayersToNotify())
                    {
                        l.OnGameStateChanged(this, args);
                    }
                }
                else if (change is PlayerStateChange)
                {
                    var castedChange = (change as PlayerStateChange);
                    State.UpdatePlayer(castedChange);

                    var args = new Event.PlayerChangedEventArgs(castedChange);
                    foreach (var l in PlayersToNotify())
                    {
                        l.OnPlayerChanged(this, args);
                    }
                }
                else if (change is UnitStateChange)
                {
                    var castedChange = (change as UnitStateChange);
                    State.UpdateUnit(castedChange);

                    var args = new Event.UnitChangedEventArgs(castedChange);
                    foreach(var l in PlayersToNotify())
                    {
                        l.OnUnitChanged(this, args);
                    }
                }
                else if (change is TerrainStateChange)
                {
                    var castedChange = (change as TerrainStateChange);
                    State.UpdateTerrain(castedChange);

                    var args = new Event.TerrainChangedEventArgs(castedChange);
                    foreach(var l in PlayersToNotify())
                    {
                        l.OnTerrainChanged(this, args);
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unsupported StateChange type of {0}", change.GetType()));
                }
            }
        }

        public override void StartGame(int height, int width, Terrain[] terrain, Unit[] units, Player[] players, Dictionary<string, object> gameStateAttributes)
        {
            State.StartGame(height, width, terrain, units, players, gameStateAttributes);

            var args = new Event.GameStartEventArgs(new GameStarted(height, width, terrain, units, players, gameStateAttributes));

            foreach (var l in PlayersToNotify())
            {
                l.OnGameStart(this, args);    
            }
        }

        private IEnumerable<IPlayerLogic> PlayersToNotify()
        {
            var list = new List<IPlayerLogic>();

            foreach (var kp in PlayerList)
            {
                foreach (var value in kp.Value.Logic)
                {
                    yield return value;
                }
            }
        }
    }
}
