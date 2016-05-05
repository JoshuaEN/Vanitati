using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game.ActionTypes;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Game
{
    public class LocalGameLogic : GameLogic
    {
        #region Properties

        protected Dictionary<int, UserSet> UserList { get; } = new Dictionary<int, UserSet>();

        public override IReadOnlyDictionary<int, User> Users
        {
            get { return UserList.ToDictionary(kp => kp.Key, kp => kp.Value.State); }
        }

        protected Dictionary<int, int?> _commanderAssignments { get; } = new Dictionary<int, int?>();

        public override IReadOnlyDictionary<int, int?> CommanderAssignments
        {
            get { return _commanderAssignments; }
        }

        public override IReadOnlyBattleGameState State
        {
            get { return InternalState; }
        }

        private BattleGameState _state;
        public BattleGameState InternalState
        {
            get { return _state; }
            set
            {
                Contract.Requires<ArgumentNullException>(null != value);
                _state = value;
            }
        }

        #endregion

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
            foreach(var key in _commanderAssignments.Keys.ToList())
            {
                var value = _commanderAssignments[key];
                if(value == userID)
                {
                    _commanderAssignments[key] = null;
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

        public void AssignUserToCommander(int? userID, int commanderID, bool isHost)
        {
            Contract.Requires<ArgumentException>(userID == null || Users.ContainsKey((int)userID));

            int? currentUserID;
            if(_commanderAssignments.TryGetValue(commanderID, out currentUserID))
            {
                if(currentUserID != null && isHost != true)
                {
                    // TODO Better exception
                    throw new Exceptions.NotHostException();
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
                throw new Exceptions.UnknownCommanderException();
            }
        }

        public override void DoActions(List<ActionInfo> actions)
        {
            DoActions(actions, true);
        }

        private void DoActions(List<ActionInfo> actions, bool external)
        {
            if (actions.Count < 1)
                return;

            {
                var args = new Event.ActionsTakenEventArgs(actions, external);
                foreach (var l in UsersToNotify())
                {
                    l.OnActionsTaken(this, args);
                }
            }
            foreach (var action in actions)
            {
                DoActionChecks(action, external);

                var changes = action.Type.PerformOn(InternalState, action.Context);

                Unit unit = null;

                if (action.Context.ActionCategory == ActionType.Category.Unit && action.Context.TriggeredManuallyByUser)
                {
                    unit = State.GetUnit((action.Context.Source as UnitContext).Location);
                }

                ApplyChanges(action, changes);

                if (null != unit)
                {
                    // If the unit is destroyed or something, don't trigger the action.
                    if (State.GetUnit(unit.UnitID) != null)
                    {

                        var triggeredActions = unit.UnitType.Actions.
                            Where(a => a.Triggers.HasFlag(UnitAction.ActionTriggers.ActionPerformedByUser)).
                            Select(a =>
                                new ActionInfo(a, new ActionContext(null, UnitAction.ActionTriggers.ActionPerformedByUser, new UnitContext(unit.Location), new GenericContext(action)))
                            );

                        if (triggeredActions.Count() > 0)
                            DoActions(triggeredActions.ToList(), false);
                    }
                }
            }
        }

        public override void DoAction(ActionInfo action)
        {
            DoAction(action, true);
        }

        private void DoAction(ActionInfo action, bool external)
        {
            DoActions(new List<ActionInfo>(1) { action }, external);
        }

        public override void Sync(int syncID)
        {
            var fields = InternalState.GetFields();
            var args = new Event.SyncEventArgs(syncID, fields);
            NotifyOfSync(args);
        }

        public void Sync(int syncID, BattleGameState.Fields fields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);

            Sync(fields);
            NotifyOfSync(new Event.SyncEventArgs(syncID, fields));
        }

        private void Sync(BattleGameState.Fields fields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);

            InternalState.Sync(fields);
            _commanderAssignments.Clear();
            foreach (var commander in fields.Commanders)
            {
                _commanderAssignments.Add(commander.CommanderID, null);
            }
        }

        private void NotifyOfSync(Event.SyncEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);

            foreach(var l in UsersToNotify())
            {
                l.OnSync(this, args);
            }
        }

        public override void StartGame(BattleGameState.Fields fields)
        {
            Sync(fields);
            InternalState.StartGame(fields);

            var args = new Event.GameStartEventArgs(fields);

            foreach (var l in UsersToNotify())
            {
                l.OnGameStart(this, args);    
            }

            var args2 = new Event.TurnChangedEventArgs(-1, new TurnChanged(-1, State.TurnID, -1, State.CurrentCommander.CommanderID, TurnChanged.Cause.GameStart));
            foreach (var l in UsersToNotify())
            {
                l.OnTurnChanged(this, args2);
            }

            DoActions(ActionsForTurnStart(State.CurrentCommander.CommanderID).ToList(), false);
        }

        public void EndTurn(int commanderID)
        {
            var previousTurnID = InternalState.TurnID;
            InternalState.EndTurn(commanderID);

            var args = new Event.TurnChangedEventArgs(previousTurnID, new TurnChanged(previousTurnID, State.TurnID, commanderID, State.CurrentCommander.CommanderID, TurnChanged.Cause.TurnEnded));
            foreach(var l in UsersToNotify())
            {
                l.OnTurnChanged(this, args);
            }

            // Ensure that the end of turn actions are all run first, followed by the start of turn actions.
            DoActions(
                ActionsForTurnEnd(commanderID).Concat(
                    ActionsForTurnStart(State.CurrentCommander.CommanderID)
                    ).ToList(),
                false
            );
        }

        public override bool IsUserCommanding(int userID, int commanderID)
        {
            int? outUserID;
            if (CommanderAssignments.TryGetValue(commanderID, out outUserID))
                return userID == outUserID;
            else
                return false;
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

        private void DoActionChecks(ActionInfo action, bool external)
        {
            if (external == true)
            {
                var triggeredByCommanderID = action.Context.TriggeredByCommanderID;
                if (triggeredByCommanderID == null)
                    throw new Exceptions.IllegalExternalActionCall();
                else if (State.SafeGetCommander((int)triggeredByCommanderID) == null)
                    throw new Exceptions.IllegalExternalActionCall();
                else if (State.CurrentCommander?.CommanderID != triggeredByCommanderID)
                    throw new Exceptions.IllegalExternalActionCall();
                if (action.Type.CanUserTrigger == false)
                    throw new Exceptions.IllegalExternalActionCall();
                else if (action.Context.ActionCategory == ActionType.Category.Game || action.Type.ActionCategory == ActionType.Category.Game)
                    throw new Exceptions.IllegalExternalActionCall();
                else if(action.Context.ActionCategory != action.Type.ActionCategory)
                    throw new Exceptions.IllegalExternalActionCall();
                else if(action.Context.ActionTargetCategory != action.Type.ActionTargetCategory)
                    throw new Exceptions.IllegalExternalActionCall();

                var source = action.Context.Source;

                if (source is UnitContext)
                {
                    var unit = State.GetUnit((source as UnitContext).Location);

                    if (unit == null)
                        throw new Exceptions.IllegalExternalActionCall();
                    else if (unit.CommanderID != triggeredByCommanderID)
                        throw new Exceptions.IllegalExternalActionCall();
                }
                else if (source is TerrainContext)
                {
                    var terrain = State.GetTerrain((source as TerrainContext).Location);

                    if (terrain == null)
                        throw new Exceptions.IllegalExternalActionCall();

                    if (terrain.TerrainType.CanCapture == false && State.GetUnit(terrain.Location)?.CommanderID != triggeredByCommanderID)
                        throw new Exceptions.IllegalExternalActionCall();
                    else if (terrain.IsOwned == false || terrain.CommanderID != triggeredByCommanderID)
                        throw new Exceptions.IllegalExternalActionCall();
                }
                else if (source is CommanderContext)
                {
                    var commander = State.SafeGetCommander((source as CommanderContext).CommanderID);

                    if (commander == null)
                        throw new Exceptions.IllegalExternalActionCall();
                    else if (commander.CommanderID != triggeredByCommanderID)
                        throw new Exceptions.IllegalExternalActionCall();
                }
                else
                {
                    throw new Exceptions.IllegalExternalActionCall($"Unknown or unsupported source context of type {source.GetType()}");
                }
            }
        }

        private void ApplyChanges(ActionInfo actionInfo, IEnumerable<StateChange> changes)
        { 
            foreach(var change in changes)
            {
                if (change is GameStateChange)
                {
                    var castedChange = (change as GameStateChange);
                    InternalState.UpdateGame(castedChange);

                    var args = new Event.GameStateChangedArgs(State.TurnID, castedChange);
                    foreach (var l in UsersToNotify())
                    {
                        l.OnGameStateChanged(this, args);
                    }
                }
                else if (change is CommanderStateChange)
                {
                    var castedChange = (change as CommanderStateChange);
                    InternalState.UpdateCommander(castedChange);

                    var args = new Event.CommanderChangedEventArgs(State.TurnID, castedChange);
                    foreach (var l in UsersToNotify())
                    {
                        l.OnCommanderChanged(this, args);
                    }
                }
                else if (change is UnitStateChange)
                {
                    var castedChange = (change as UnitStateChange);
                    Unit unit = State.GetUnit(castedChange.UnitID);


                    InternalState.UpdateUnit(castedChange);

                    var args = new Event.UnitChangedEventArgs(State.TurnID, castedChange);
                    foreach(var l in UsersToNotify())
                    {
                        l.OnUnitChanged(this, args);
                    }

                    if(castedChange.ChangeCause != UnitStateChange.Cause.Destroyed)
                    {
                        if (castedChange.ChangeCause == UnitStateChange.Cause.Created)
                            unit = State.GetUnit(castedChange.UnitID);

                        var actions = unit.UnitType.Actions.
                            Where(a => a.Triggers.HasFlag(UnitAction.ActionTriggers.PropertyChanged)).
                            Select(a =>
                                new ActionInfo(a, new ActionContext(null, UnitAction.ActionTriggers.PropertyChanged, new UnitContext(unit.Location), new GenericContext(castedChange.UpdatedProperties)))
                            );

                        if (actions.Count() > 0)
                            DoActions(actions.ToList(), false);
                    }

                    if(castedChange.ChangeCause == UnitStateChange.Cause.Created || castedChange.ChangeCause == UnitStateChange.Cause.Destroyed)
                    {
                        Contract.Assert(unit != null);

                        IEnumerable<ActionType> actions = null;
                        UnitAction.ActionTriggers trigger = UnitAction.ActionTriggers.ManuallyByUser;

                        if (castedChange.ChangeCause == UnitStateChange.Cause.Created)
                        {
                            trigger = UnitAction.ActionTriggers.UnitCreated;
                        }
                        else if (castedChange.ChangeCause == UnitStateChange.Cause.Destroyed)
                        {
                            trigger = UnitAction.ActionTriggers.UnitDestroyed;
                        }
                        else
                        {
                            Contract.Assert(false, "Should not be reached");
                        }

                        actions = unit.UnitType.Actions.Where(a => a.Triggers.HasFlag(trigger));

                        Contract.Assert(null != actions);
                        Contract.Assert(trigger != UnitAction.ActionTriggers.ManuallyByUser);

                        var unitContext = new UnitContext(unit.Location);
                        DoActions(actions.Select(a => new ActionInfo(a, new ActionContext(null, trigger, unitContext, unitContext))).ToList(), false);
                    }
                }
                else if (change is TerrainStateChange)
                {
                    var castedChange = (change as TerrainStateChange);
                    InternalState.UpdateTerrain(castedChange);

                    var args = new Event.TerrainChangedEventArgs(State.TurnID, castedChange);
                    foreach(var l in UsersToNotify())
                    {
                        l.OnTerrainChanged(this, args);
                    }
                }
                else if(change is TurnEnd)
                {
                    EndTurn((change as TurnEnd).CommanderID);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unsupported StateChange type of {0}", change.GetType()));
                }
            }
        }

        private IEnumerable<ActionInfo> ActionsForTurnStart(int currentTurnCommanderID)
        {
            foreach (var unit in State.Units.Values.Where(u => u.CommanderID == currentTurnCommanderID))
            {
                var unitContext = new UnitContext(unit.Location);
                var otherContext = new OtherContext();
                foreach (var action in unit.UnitType.Actions.Where(u => u.Triggers.HasFlag(UnitAction.ActionTriggers.TurnStart)))
                {
                    yield return new ActionInfo(action, new ActionContext(null, UnitAction.ActionTriggers.TurnStart, unitContext, otherContext));
                }
            }

            foreach (var terrain in State.Terrain)
            {
                foreach (var action in terrain.TerrainType.Actions)
                {
                    var terrainContext = new TerrainContext(terrain.Location);
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.AnyTurnStart))
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.AnyTurnStart, terrainContext, new OtherContext()));
                    }
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.OccupyingUnitTurnStart) &&
                             State.GetTile(terrain.Location)?.Unit?.CommanderID == currentTurnCommanderID)
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.OccupyingUnitTurnStart, terrainContext, new UnitContext(terrain.Location)));
                    }
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.OwnerTurnStart) &&
                             terrain.IsOwned == true &&
                             terrain.CommanderID == currentTurnCommanderID)
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.OwnerTurnStart, terrainContext, terrainContext));
                    }
                }
            }

            foreach(var action in State.GetCommander(currentTurnCommanderID).CommanderType.Actions)
            {
                var commanderContext = new CommanderContext(currentTurnCommanderID);
                if (action.Triggers.HasFlag(CommanderAction.ActionTriggers.TurnStart))
                    yield return new ActionInfo(action, new ActionContext(null, CommanderAction.ActionTriggers.TurnStart, commanderContext, new OtherContext()));
            }
        }

        private IEnumerable<ActionInfo> ActionsForTurnEnd(int currentTurnCommanderID)
        {
            foreach (var unit in State.Units.Values.Where(u => u.CommanderID == currentTurnCommanderID))
            {
                var unitContext = new UnitContext(unit.Location);
                foreach (var action in unit.UnitType.Actions.Where(u => u.Triggers.HasFlag(UnitAction.ActionTriggers.TurnEnd)))
                {
                    yield return new ActionInfo(action, new ActionContext(null, UnitAction.ActionTriggers.TurnEnd, unitContext, unitContext));
                }
            }

            foreach (var terrain in State.Terrain)
            {
                foreach (var action in terrain.TerrainType.Actions)
                {
                    var terrainContext = new TerrainContext(terrain.Location);
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.AnyTurnEnd))
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.AnyTurnEnd, terrainContext, new OtherContext()));
                    }
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.OccupyingUnitTurnEnd) &&
                             State.GetTile(terrain.Location)?.Unit?.CommanderID == currentTurnCommanderID)
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.OccupyingUnitTurnEnd, terrainContext, new UnitContext(terrain.Location)));
                    }
                    if (action.Triggers.HasFlag(TerrainAction.ActionTriggers.OwnerTurnEnd) &&
                             terrain.IsOwned == true &&
                             terrain.CommanderID == currentTurnCommanderID)
                    {
                        yield return new ActionInfo(action, new ActionContext(null, TerrainAction.ActionTriggers.OwnerTurnEnd, terrainContext, terrainContext));
                    }
                }
            }

            foreach (var action in State.GetCommander(currentTurnCommanderID).CommanderType.Actions)
            {
                var commanderContext = new CommanderContext(currentTurnCommanderID);
                if (action.Triggers.HasFlag(CommanderAction.ActionTriggers.TurnEnd))
                    yield return new ActionInfo(action, new ActionContext(null, CommanderAction.ActionTriggers.TurnEnd, commanderContext, new OtherContext()));
            }
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != CommanderAssignments);
            Contract.Invariant(null != _commanderAssignments);
            Contract.Invariant(null != UserList);
            Contract.Invariant(null != State);
            Contract.Invariant(null != _state);
            Contract.Invariant(null != InternalState);
        }
    }
}
