using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Game
{
    public class LocalGameLogic : GameLogic
    {
        protected Dictionary<int, UserSet> UserList { get; } = new Dictionary<int, UserSet>();
        protected Dictionary<int, int?> _commanderAssignments { get; } = new Dictionary<int, int?>();

        public override IReadOnlyDictionary<int, int?> CommanderAssignments
        {
            get { return _commanderAssignments; }
        }

        public override IReadOnlyBattleGameState State
        {
            get
            {
                return InternalState;
            }
        }

        private BattleGameState _state;
        public BattleGameState InternalState
        {
            get
            {
                return _state;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(null != value);
                _state = value;
            }
        }

        public LocalGameLogic()
        {
            InternalState = new BattleGameState();
        }

        public override void AddUser(User user, IReadOnlyList<IUserLogic> logic)
        {
            UserList.Add(user.UserID, new UserSet(logic, user));
            var args = new Event.UserAddedEventArgs(user);
            foreach (var l in UsersToNotify())
            {
                l.OnUserAdded(this, args);
            }
        }

        public override void RemoveUser(int userID)
        {
            UserList.Remove(userID);
            foreach(var kp in _commanderAssignments)
            {
                if(kp.Value == userID)
                {
                    _commanderAssignments[kp.Key] = null;
                }
            }
            var args = new Event.UserRemovedEventArgs(userID);
            foreach (var l in UsersToNotify())
            {
                l.OnUserRemoved(this, args);
            }
        }

        public override void AssignUserToCommander(int? userID, int commanderID)
        {
            AssignUserToCommander(userID, commanderID, false);
        }

        public void AssignUserToCommander(int? userID, int commanderID, bool isHost = false)
        {
            int? currentUserID;
            if(_commanderAssignments.TryGetValue(commanderID, out currentUserID))
            {
                if(currentUserID != null && isHost != true)
                {
                    // TODO Better exception
                    throw new NotSupportedException();
                }
                _commanderAssignments[commanderID] = userID;

                var args = new Event.UserAssignedToCommanderEventArgs(userID, commanderID);
                foreach (var l in UsersToNotify())
                {
                    l.OnUserAssignedToCommander(this, args);
                }
            }
            else
            {
                // TODO Better exception
                throw new NotSupportedException("Invalid commander ID");
            }
        }

        public override void DoAction(ActionInfo action)
        {
            var changes = action.Type.PerformOn(
                InternalState, 
                new Action.ActionContext(action.CommanderID, action.Trigger), 
                InternalState.GetTile(action.Source), InternalState.GetTile(action.Target));

            foreach(var change in changes)
            {
                if (change is GameStateChange)
                {
                    var castedChange = (change as GameStateChange);
                    InternalState.UpdateGame(castedChange);

                    var args = new Event.GameStateChangedArgs(castedChange);
                    foreach (var l in UsersToNotify())
                    {
                        l.OnGameStateChanged(this, args);
                    }
                }
                else if (change is CommanderStateChange)
                {
                    var castedChange = (change as CommanderStateChange);
                    InternalState.UpdateCommander(castedChange);

                    var args = new Event.CommanderChangedEventArgs(castedChange);
                    foreach (var l in UsersToNotify())
                    {
                        l.OnCommanderChanged(this, args);
                    }
                }
                else if (change is UnitStateChange)
                {
                    var castedChange = (change as UnitStateChange);
                    InternalState.UpdateUnit(castedChange);

                    var args = new Event.UnitChangedEventArgs(castedChange);
                    foreach(var l in UsersToNotify())
                    {
                        l.OnUnitChanged(this, args);
                    }
                }
                else if (change is TerrainStateChange)
                {
                    var castedChange = (change as TerrainStateChange);
                    InternalState.UpdateTerrain(castedChange);

                    var args = new Event.TerrainChangedEventArgs(castedChange);
                    foreach(var l in UsersToNotify())
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

        public override void Sync(int syncID)
        {
            var fields = InternalState.GetFields();
            var args = new Event.SyncEventArgs(syncID, fields);
            NotifyOfSync(args);
        }

        public void Sync(int syncID, BattleGameState.Fields fields)
        {
            Sync(fields);
            NotifyOfSync(new Event.SyncEventArgs(syncID, fields));
        }

        private void Sync(BattleGameState.Fields fields)
        {
            InternalState.Sync(fields);
            _commanderAssignments.Clear();
            foreach (var commander in fields.Commanders)
            {
                _commanderAssignments.Add(commander.CommanderID, null);
            }
        }

        private void NotifyOfSync(Event.SyncEventArgs args)
        {
            foreach(var l in UsersToNotify())
            {
                l.OnSync(this, args);
            }
        }

        public override void StartGame(BattleGameState.Fields fields)
        {
            InternalState.StartGame(fields);
            Sync(fields);

            var args = new Event.GameStartEventArgs(fields);

            foreach (var l in UsersToNotify())
            {
                l.OnGameStart(this, args);    
            }
        }

        public override void EndTurn(int commanderID)
        {
            InternalState.EndTurn(commanderID);

            var args = new Event.TurnEndedEventArgs(commanderID);
            foreach(var l in UsersToNotify())
            {
                l.OnTurnEnded(this, args);
            }

            // Ensure that the end of turn actions are all run first, followed by the start of turn actions.
            DoActions(
                CommanderUnitActionsForTrigger(commanderID, ActionType.ActionTriggers.TurnEnd).Concat(
                CommanderUnitActionsForTrigger(State.CurrentCommander.CommanderID, ActionType.ActionTriggers.TurnStart)
                ).ToList()
            );
        }

        private IEnumerable<IUserLogic> UsersToNotify()
        {
            foreach (var kp in UserList)
            {
                foreach (var value in kp.Value.Logic)
                {
                    yield return value;
                }
            }
        }

        private IEnumerable<ActionInfo> CommanderUnitActionsForTrigger(int commanderID, ActionType.ActionTriggers trigger)
        {
            foreach (var unit in State.Units.Values)
            {
                if (unit.CommanderID != commanderID)
                    continue;

                foreach(var action in unit.UnitType.Actions)
                {
                    var matches = false;
                    switch(trigger)
                    {
                        case ActionType.ActionTriggers.AttributeChange:
                            matches = action.TriggerOnAttributeChange;
                            break;
                        case ActionType.ActionTriggers.TurnEnd:
                            matches = action.TriggerOnTurnEnd;
                            break;
                        case ActionType.ActionTriggers.TurnStart:
                            matches = action.TriggerOnTurnStart;
                            break;
                        case ActionType.ActionTriggers.UnitCreated:
                            matches = action.TriggerOnUnitCreated;
                            break;
                        case ActionType.ActionTriggers.UnitDestroyed:
                            matches = action.TriggerOnUnitDestroyed;
                            break;
                        default:
                            throw new ArgumentException(string.Format("Unsupported ActionTrigger {0}", trigger));
                    }

                    if(matches)
                    {
                        yield return new ActionInfo(unit.CommanderID, action, unit.Location, unit.Location, trigger);
                    }
                }
            }
        }
    }
}
