using Xunit;
using UnnamedStrategyGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGameTests.TestHelpers.FakeConcrete;

namespace UnnamedStrategyGame.Game.Tests
{
    public class LocalGameLogicTests
    {
        public LocalGameLogicTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void AddUserTest()
        {
            var logic = new LocalGameLogic();
            var user = new User(0, "user");
            var ilogic = new List<IUserLogic>();

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, e => Assert.Equal((e as UserAddedEventArgs).User, user)));

            logic.AddUser(user, handler);

            handler.ForEach(h => h.End());
            Assert.Equal(1, logic.Users.Count);
            Assert.Equal(user, logic.Users[user.UserID]);
        }

        [Fact()]
        public void RemoveUserTest()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var userToRemove = new User(0, "user");
            var userToWatch = new User(1, "user");

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserRemoved, e => Assert.Equal(userToRemove.UserID, (e as UserRemovedEventArgs).UserID)));

            logic.AddUser(userToRemove, new List<IUserLogic>());
            logic.AssignUserToCommander(userToRemove.UserID, 0);
            logic.AddUser(userToWatch, handler);
            handler.ForEach(h => h.Pause = true);
            logic.AssignUserToCommander(userToWatch.UserID, 1);
            handler.ForEach(h => h.Pause = false);

            logic.RemoveUser(userToRemove.UserID);

            handler.ForEach(h => h.End());
            Assert.Equal(1, logic.Users.Count);
            Assert.Equal(null, logic.CommanderAssignments[0]);
            Assert.Equal(userToWatch.UserID, logic.CommanderAssignments[1]);
        }

        [Fact()]
        public void AssignUserToCommanderTest_SetSimple()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var user = new User(0, "user");
            var cmdId = logic.State.Commanders.First().Key;

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAssignedToCommander, e => 
                {
                    var args = (e as UserAssignedToCommanderEventArgs);
                    Assert.Equal(user.UserID, args.UserID);
                    Assert.Equal(cmdId, args.CommanderID);
                })
            );

            logic.AddUser(user, handler);

            logic.AssignUserToCommander(user.UserID, cmdId);

            handler.ForEach(h => h.End());
            Assert.Equal(user.UserID, logic.CommanderAssignments[cmdId]);
        }

        [Fact()]
        public void AssignUserToCommanderTest_SetOverwrite()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var userA = new User(0, "user");
            var userB = new User(1, "user", true);
            var cmdId = logic.State.Commanders.First().Key;

            var handlerA = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAssignedToCommander, e =>
                {
                    var args = (e as UserAssignedToCommanderEventArgs);
                    Assert.Equal(userA.UserID, args.UserID);
                    Assert.Equal(cmdId, args.CommanderID);
                }),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAssignedToCommander, e =>
                {
                    var args = (e as UserAssignedToCommanderEventArgs);
                    Assert.Equal(userB.UserID, args.UserID);
                    Assert.Equal(cmdId, args.CommanderID);
                })
            );

            var handlerB = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAssignedToCommander, e =>
                {
                    var args = (e as UserAssignedToCommanderEventArgs);
                    Assert.Equal(userA.UserID, args.UserID);
                    Assert.Equal(cmdId, args.CommanderID);
                }),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAssignedToCommander, e =>
                {
                    var args = (e as UserAssignedToCommanderEventArgs);
                    Assert.Equal(userB.UserID, args.UserID);
                    Assert.Equal(cmdId, args.CommanderID);
                })
            );

            logic.AddUser(userA, handlerA);
            logic.AddUser(userB, handlerB);

            
            logic.AssignUserToCommander(userA.UserID, cmdId);
            logic.AssignUserToCommander(userB.UserID, cmdId, true);

            handlerA.ForEach(h => h.End());
            handlerB.ForEach(h => h.End());

            Assert.Equal(userB.UserID, logic.CommanderAssignments[cmdId]);
        }

        [Fact()]
        public void AssignUserToCommanderTest_SetOverwriteNotHost()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var userA = new User(0, "user");
            var userB = new User(1, "user");

            logic.AddUser(userA, new List<IUserLogic>());
            logic.AddUser(userB, new List<IUserLogic>());

            var cmdId = logic.State.Commanders.First().Key;
            logic.AssignUserToCommander(userA.UserID, cmdId);

            Assert.Throws<Exceptions.NotHostException>(() =>
            {
                logic.AssignUserToCommander(userB.UserID, cmdId);
            });

            Assert.Equal(userA.UserID, logic.CommanderAssignments[cmdId]);
        }

        [Fact()]
        public void AssignUserToCommanderTest_SetInvalidCommanderID()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var user = new User(0, "user");
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.UnknownCommanderException>(() =>
            {
                logic.AssignUserToCommander(user.UserID, 99);
            });

        }

        [Fact()]
        public void AssignUserToCommanderTest_SetInvalidUserID()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var user = new User(0, "user");
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<ArgumentException>(() =>
            {
                logic.AssignUserToCommander(99, 0);
            });

        }

        [Fact()]
        public void DoActionTest_ValidActions()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);

            var cmdID = logic.State.CurrentCommander.CommanderID;
            var dic = new Dictionary<string, object>();
            var unitID = logic.State.GetNextUnitID();
            var unitLoc = new Location(1, 0);
            var terrainLoc = new Location(0, 1);
            var commander = new FakeActionType(new List<StateChange>()
            {
                new StateChanges.CommanderStateChange(cmdID, dic)
            }, ActionType.Category.Commander);
            var unit = new FakeActionType(new List<StateChange>()
            {
                new StateChanges.UnitStateChange(unitID, new Unit(unitID, UnitTypes.Infantry.Instance, unitLoc, cmdID).GetWriteableProperties(), unitLoc, StateChanges.UnitStateChange.Cause.Created),
                new StateChanges.UnitStateChange(unitID, dic, unitLoc, StateChanges.UnitStateChange.Cause.Changed),
                new StateChanges.UnitStateChange(unitID, dic, unitLoc, StateChanges.UnitStateChange.Cause.Destroyed)
            }, ActionType.Category.Unit);
            var terrain = new FakeActionType(new List<StateChange>()
            {
                new StateChanges.TerrainStateChange(terrainLoc, dic)
            }, ActionType.Category.Terrain);
            var game = new FakeActionType(new List<StateChange>()
            {
                new StateChanges.GameStateChange(dic)
            }, ActionType.Category.Game);

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null));
            var handler0 = handler[0];
            handler0.AddCallbacks(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnActionsTaken, (e) =>
                {
                    var args = (e as ActionsTakenEventArgs);

                    Assert.Equal(1, args.Actions.Count);
                    Assert.Equal(commander, args.Actions.First().Type);
                })).
                AddCallbacks(commander.CallbacksForStateChanges().ToArray()).
                
                AddCallbacks(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnActionsTaken, (e) =>
                {
                    var args = (e as ActionsTakenEventArgs);

                    Assert.Equal(1, args.Actions.Count);
                    Assert.Equal(unit, args.Actions.First().Type);
                })).
                AddCallbacks(unit.CallbacksForStateChanges().ToArray()).
                
                AddCallbacks(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnActionsTaken, (e) =>
                {
                    var args = (e as ActionsTakenEventArgs);

                    Assert.Equal(1, args.Actions.Count);
                    Assert.Equal(terrain, args.Actions.First().Type);
                })).
                AddCallbacks(terrain.CallbacksForStateChanges().ToArray()).
                
                AddCallbacks(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnActionsTaken, (e) =>
                {
                    var args = (e as ActionsTakenEventArgs);

                    Assert.Equal(1, args.Actions.Count);
                    Assert.Equal(game, args.Actions.First().Type);
                })).
                AddCallbacks(game.CallbacksForStateChanges().ToArray());

            logic.AddUser(user, handler);

            handler.ForEach(l => l.Pause = true);
            logic.AssignUserToCommander(user.UserID, cmdID);
            handler.ForEach(l => l.Pause = false);

            var otherContext = new OtherContext();
            logic.DoActions(new List<ActionInfo>() { new ActionInfo(commander, new ActionContext(cmdID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(cmdID), otherContext)) });
            logic.DoAction(new ActionInfo(unit, new ActionContext(cmdID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new UnitContext(new Location(1, 1)), otherContext)));
            logic.DoAction(new ActionInfo(terrain, new ActionContext(cmdID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new TerrainContext(new Location(1,1)), otherContext)));

            var method = logic.GetType().GetMethod("DoAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(logic, new object[] { new ActionInfo(game, new ActionContext(cmdID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new TerrainContext(new Location(1, 1)), otherContext)), false });

            handler.ForEach(l => l.End());
        }

        [Fact()]
        public void DoActionTest_InvalidTriggeredByCommanderID_NULL()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(null, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_InvalidTriggeredByCommanderID_NotValidCommanderID()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(99, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_InvalidTriggeredByCommanderID_NotCurrentTurn()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(0, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_Invalid_UserCannotTrigger()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander, ActionType.TargetCategory.Other, false),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_ActionCategoryMismatch()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new UnitContext(new Location()), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_TargetActionCategoryMismatch()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander, ActionType.TargetCategory.Tile),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_GameActionCategory()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Game),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new GameContext(), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_UnitContext_InvalidUnitLocation()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Unit),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new UnitContext(new Location(1, 0)), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_UnitContext_UnitCommanderVsTriggerCommanderMismatch()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Unit),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new UnitContext(new Location(0, 0)), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_TerrainContext_InvalidLocation()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Terrain),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new TerrainContext(new Location(2, 2)), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_TerrainContext_NotCapturableAndNoOwnedUnitOn()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Terrain),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new TerrainContext(new Location(1, 0)), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_TerrainContext_NotOwned()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Terrain),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new TerrainContext(new Location(0, 0)), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_CommanderContext_InvalidCommander()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(99), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void DoActionTest_CommanderContext_CommanderMismatch()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            logic.StartGame(fields);
            logic.AddUser(user, new List<IUserLogic>());

            Assert.Throws<Exceptions.IllegalExternalActionCall>(() =>
            {
                logic.DoAction(
                    new ActionInfo(
                        new FakeActionType(new List<StateChange>(), ActionType.Category.Commander),
                        new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(0), new OtherContext())
                   )
                );
            });
        }

        [Fact()]
        public void SyncTest()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnSync, e =>
                {
                    var args = (e as SyncEventArgs);
                    Assert.Equal(0, args.SyncID);
                    BattleGameStateTests.CrossCheckFields(fields, args.Fields);
                })
            );

            logic.AddUser(user, handler);
            logic.Sync(0, fields);
            handler.ForEach(h => h.End());

            BattleGameStateTests.CrossCheckFieldsWithState(fields, logic.InternalState);

            Assert.Equal(fields.Commanders.Length, logic.CommanderAssignments.Count);
            Assert.Equal(fields.Commanders.Select(c => c.CommanderID).OrderBy(c => c), logic.CommanderAssignments.Keys.OrderBy(c => c));
        }

        [Fact()]
        public void SyncTest_NoFieldGiven()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            logic.Sync(0, fields);
            var user = new User(0, "name");

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnSync, e =>
                {
                    var args = (e as SyncEventArgs);
                    Assert.Equal(0, args.SyncID);
                    BattleGameStateTests.CrossCheckFields(fields, args.Fields);
                })
            );

            logic.AddUser(user, handler);
            logic.Sync(0);
            handler.ForEach(h => h.End());

            BattleGameStateTests.CrossCheckFieldsWithState(fields, logic.InternalState);

            Assert.Equal(fields.Commanders.Length, logic.CommanderAssignments.Count);
            Assert.Equal(fields.Commanders.Select(c => c.CommanderID).OrderBy(c => c), logic.CommanderAssignments.Keys.OrderBy(c => c));
        }

        [Fact()]
        public void StartGameTest()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnGameStart, e =>
                {
                    var args = (e as GameStartEventArgs);
                    BattleGameStateTests.CrossCheckFields(fields, args.ChangeInfo);
                }),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnTurnChanged, e =>
                {
                    var args = (e as TurnChangedEventArgs);
                    Assert.Equal(StateChanges.TurnChanged.Cause.GameStart, args.ChangeInfo.ChangeCause);
                    Assert.Equal(-1, args.TurnID);
                    Assert.Equal(logic.State.TurnID, args.ChangeInfo.NewTurnID);
                    Assert.Equal(-1, args.ChangeInfo.PreviousTurnID);
                    Assert.Equal(-1, args.ChangeInfo.PreviousCommanderID);
                    Assert.Equal(1, args.ChangeInfo.NextCommanderID);
                }),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.EndOfHandlers, null)
            );

            logic.AddUser(user, handler);
            logic.StartGame(fields);
            handler.ForEach(h => h.End());

            fields = new BattleGameState.Fields(fields.Height, fields.Width, fields.Terrain, fields.Units, fields.Commanders, fields.Values, 1);
            BattleGameStateTests.CrossCheckFieldsWithState(fields, logic.InternalState);

            Assert.Equal(fields.Commanders.Length, logic.CommanderAssignments.Count);
            Assert.Equal(fields.Commanders.Select(c => c.CommanderID).OrderBy(c => c), logic.CommanderAssignments.Keys.OrderBy(c => c));
        }

        [Fact()]
        public void EndTurnTest_Valid()
        {
            var logic = new LocalGameLogic();
            var fields = BattleGameStateTests.GetFields();
            var user = new User(0, "name");

            var handler = GetIUserLogic(new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUserAdded, null),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnTurnChanged, e =>
                {
                    var args = (e as TurnChangedEventArgs);
                    Assert.Equal(StateChanges.TurnChanged.Cause.TurnEnded, args.ChangeInfo.ChangeCause);
                    Assert.Equal(logic.State.TurnID - 1, args.TurnID);
                    Assert.Equal(logic.State.TurnID, args.ChangeInfo.NewTurnID);
                    Assert.Equal(logic.State.TurnID - 1, args.ChangeInfo.PreviousTurnID);
                    Assert.Equal(1, args.ChangeInfo.PreviousCommanderID);
                    Assert.Equal(0, args.ChangeInfo.NextCommanderID);
                }),
                new FakeIUserLogic.Callback(FakeIUserLogic.Handler.EndOfHandlers, null)
            );

            logic.AddUser(user, handler);
            handler.ForEach(h => h.Pause = true);
            logic.StartGame(fields);
            handler.ForEach(h => h.Pause = false);
            logic.EndTurn(logic.State.CurrentCommander.CommanderID);
            handler.ForEach(h => h.End());
        }

        [Fact()]
        public void IsUserCommandingTest()
        {
            var logic = new LocalGameLogic();
            logic.Sync(0, BattleGameStateTests.GetFields());
            var user = new User(0, "user");
            logic.AddUser(user, new List<IUserLogic>());

            var cmdId = logic.State.Commanders.First().Key;
            logic.AssignUserToCommander(user.UserID, cmdId);

            Assert.True(logic.IsUserCommanding(user.UserID, cmdId));
            Assert.False(logic.IsUserCommanding(user.UserID + 1, cmdId));
            Assert.False(logic.IsUserCommanding(user.UserID , 99));
        }

        public static List<FakeIUserLogic> GetIUserLogic(params FakeIUserLogic.Callback[] callbacks)
        {
            return new List<FakeIUserLogic>()
            {
                new FakeIUserLogic(callbacks)
            };
        }

        public class FakeActionType : ActionType
        {
            public List<StateChange> TestStateChanges { get; }

            public override Category ActionCategory
            {
                get;
            }

            public override TargetCategory ActionTargetCategory
            {
                get;
            }

            public override bool CanUserTrigger
            {
                get;
            }

            public FakeActionType(List<StateChange> testStateChanges, Category actionCategory, TargetCategory actionTargetCategory = TargetCategory.Other, bool canUserTrigger = true) : base("")
            {
                TestStateChanges = testStateChanges;
                ActionCategory = actionCategory;
                ActionTargetCategory = actionTargetCategory;
                CanUserTrigger = canUserTrigger;
            }

            public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
            {
                return true;
            }

            public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
            {
                return TestStateChanges.ToList();
            }

            public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
            {
                return new List<Modifier>(0);
            }

            public List<FakeIUserLogic.Callback> CallbacksForStateChanges()
            {
                var list = new List<FakeIUserLogic.Callback>(TestStateChanges.Count);
                foreach(var stateChange in TestStateChanges)
                {
                    list.Add(CallbackForStateChange(stateChange));
                }

                return list;
            }

            public FakeIUserLogic.Callback CallbackForStateChange(StateChange stateChange)
            {
                if(stateChange is StateChanges.CommanderStateChange)
                {
                    var change = (stateChange as StateChanges.CommanderStateChange);
                    return new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnCommanderChanged, e =>
                    {
                        var args = (e as CommanderChangedEventArgs);

                        Assert.Equal(change.ChangeCause, args.ChangeInfo.ChangeCause);
                        Assert.Equal(change.CommanderID, args.ChangeInfo.CommanderID);
                        Assert.Equal(change.UpdatedProperties, args.ChangeInfo.UpdatedProperties);
                    });
                }
                else if(stateChange is StateChanges.TerrainStateChange)
                {
                    var change = (stateChange as StateChanges.TerrainStateChange);
                    return new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnTerrainChanged, e =>
                    {
                        var args = (e as TerrainChangedEventArgs);

                        Assert.Equal(change.Location, args.ChangeInfo.Location);
                        Assert.Equal(change.UpdatedProperties, args.ChangeInfo.UpdatedProperties);
                    });
                }
                else if (stateChange is StateChanges.UnitStateChange)
                {
                    var change = (stateChange as StateChanges.UnitStateChange);
                    return new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnUnitChanged, e =>
                    {
                        var args = (e as UnitChangedEventArgs);

                        Assert.Equal(change.Location, args.ChangeInfo.Location);
                        Assert.Equal(change.ChangeCause, args.ChangeInfo.ChangeCause);
                        Assert.Equal(change.PreviousLocation, args.ChangeInfo.PreviousLocation);
                        Assert.Equal(change.UnitID, args.ChangeInfo.UnitID);
                        Assert.Equal(change.UpdatedProperties, args.ChangeInfo.UpdatedProperties);
                    });
                }
                else if (stateChange is StateChanges.TurnEnd)
                {
                    var change = (stateChange as StateChanges.TurnEnd);
                    return new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnTurnChanged, e =>
                    {
                        var args = (e as TurnChangedEventArgs);

                        Assert.Equal(change.CommanderID, args.ChangeInfo.PreviousCommanderID);
                        Assert.Equal(StateChanges.TurnChanged.Cause.TurnEnded, args.ChangeInfo.ChangeCause);
                    });
                }
                else if(stateChange is StateChanges.GameStateChange)
                {
                    var change = (stateChange as StateChanges.GameStateChange);
                    return new FakeIUserLogic.Callback(FakeIUserLogic.Handler.OnGameStateChanged, e =>
                    {
                        var args = (e as GameStateChangedArgs);

                        Assert.Equal(change.UpdatedProperties, args.ChangeInfo.UpdatedProperties);
                    });
                }
                else
                {
                    throw new NotSupportedException($"Unknown state change type of {stateChange}");
                }
            }
        }
    }
}