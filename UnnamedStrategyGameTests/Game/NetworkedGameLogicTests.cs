using Xunit;
using UnnamedStrategyGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Network;
using UnnamedStrategyGameTests.TestHelpers.FakeConcrete;
using UnnamedStrategyGame.Network.MessageWrappers;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.Tests
{
    public class NetworkedGameLogicTests
    {
        public NetworkedGameLogicTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void NetworkedGameLogicTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "", handler);

            Assert.Equal(client, logic.Client);
        }

        [Fact()]
        public void ListenTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);

            logic.Listen();

            Assert.Equal(1, client.SentMessages.Count);
            Assert.True(logic.Listening);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(ClientHelloProtocolWrapper), args.GetType());

            var parsedArgs = (args as ClientHelloProtocolWrapper);
            Assert.Equal("name", parsedArgs.ClientHelloData.Name);
        }

        [Fact()]
        public void DoActionTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            logic.Logic.StartGame(BattleGameStateTests.GetFields());

            client.SentMessages.Clear();

            var action = new ActionInfo(ActionTypes.ForUnits.Move.Instance, new ActionContext(1, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new UnitContext(new Location(1, 1)), new GenericContext(new Location(0,1))));
            logic.DoActions(new List<ActionInfo>() { action });

            Assert.Equal(1, client.SentMessages.Count);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(DoActionsCallWrapper), args.GetType());
            var parsedArgs = (args as DoActionsCallWrapper);

            Assert.Equal(action, parsedArgs.Actions.First());

#if NETWORK_PREDICTION
            Assert.Equal(new Location(0, 1), logic.State.GetUnit(1).Location);
#endif
        }

        [Fact()]
        public void SyncTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            logic.Sync(50);

            Assert.Equal(1, client.SentMessages.Count);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(SyncCallWrapper), args.GetType());
            var parsedArgs = (args as SyncCallWrapper);

            Assert.Equal(50, parsedArgs.SyncID);
        }

        [Fact()]
        public void StartGameTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            var fields = BattleGameStateTests.GetFields();
            logic.StartGame(fields);

            Assert.Equal(1, client.SentMessages.Count);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(StartGameCallWrapper), args.GetType());
            var parsedArgs = (args as StartGameCallWrapper);

            Assert.Equal(fields, parsedArgs.ChangeInfo);
        }

        [Fact()]
        public void AddUserTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);

            Assert.Throws<NotSupportedException>(() =>
            {
                logic.AddUser(new User(0, "hello"), new List<IUserLogic>());
            });
        }

        [Fact()]
        public void RemoveUserTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            logic.RemoveUser(1);

            Assert.Equal(1, client.SentMessages.Count);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(RemoveUserCallWrapper), args.GetType());
            var parsedArgs = (args as RemoveUserCallWrapper);

            Assert.Equal(1, parsedArgs.UserID);
        }

        [Fact()]
        public void AssignUserToCommanderTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            logic.AssignUserToCommander(10, 21);

            Assert.Equal(1, client.SentMessages.Count);

            var args = client.SentMessages.Dequeue();
            Assert.Equal(typeof(AssignUserToCommanderCallWrapper), args.GetType());
            var parsedArgs = (args as AssignUserToCommanderCallWrapper);

            Assert.Equal(10, parsedArgs.UserID);
            Assert.Equal(21, parsedArgs.CommanderID);
        }

        [Fact()]
        public void OnActionsTakenTest()
        {
            var logic = GetLogicForEvents();
            logic.OnActionsTaken(this, new Event.ActionsTakenEventArgs(new List<ActionInfo>(), true));
            Assert.True(true, "This method does not currently do anything, it is simply needed to satisfy an interface");
        }

        [Fact()]
        public void OnCommanderChangedTest_Applied()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetCommander(0).Credits;
            var after = before + 9;

            logic.OnCommanderChanged(this,
                new Event.CommanderChangedEventArgs(
                    logic.State.TurnID + 1,
                     null,
                    new StateChanges.CommanderStateChange(0, new Dictionary<string, object>() { { "Credits", before + after } })));

            Assert.Equal(after, logic.State.GetCommander(0).Credits);
        }

        [Fact()]
        public void OnCommanderChangedTest_ExcludedByPrediction()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetCommander(0).Credits;

            logic.OnCommanderChanged(this, 
                new Event.CommanderChangedEventArgs(
                    logic.State.TurnID,
                     null,
                    new StateChanges.CommanderStateChange(0, new Dictionary<string, object>() { { "Credits", before + 99999 } })));

            Assert.Equal(before, logic.State.GetCommander(0).Credits);

        }

        [Fact()]
        public void OnUnitChangedTest_Applied()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetUnit(0).Health;
            var after = before - 1;

            logic.OnUnitChanged(this,
                new Event.UnitChangedEventArgs(
                    logic.State.TurnID + 1,
                    null,
                    new StateChanges.UnitStateChange(0, new Dictionary<string, object>() { { "Health", after } }, logic.State.GetUnit(0).Location)));

            Assert.Equal(after, logic.State.GetUnit(0).Health);
        }

        [Fact()]
        public void OnUnitChangedTest_ExcludedByPrediction()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetUnit(0).Health;
            var after = before - 1;

            logic.OnUnitChanged(this,
                new Event.UnitChangedEventArgs(
                    logic.State.TurnID,
                    null,
                    new StateChanges.UnitStateChange(0, new Dictionary<string, object>() { { "Health", after } }, logic.State.GetUnit(0).Location)));

            Assert.Equal(before, logic.State.GetUnit(0).Health);
        }

        [Fact()]
        public void OnTerrainChangedTest_Applied()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetTerrain(0, 0).IsOwned;
            var after = !before;

            logic.OnTerrainChanged(this,
                new Event.TerrainChangedEventArgs(
                    logic.State.TurnID + 1,
                     null,
                    new StateChanges.TerrainStateChange(new Location(0, 0), new Dictionary<string, object>() { { "IsOwned", after } })));

            Assert.Equal(after, logic.State.GetTerrain(0, 0).IsOwned);
        }

        [Fact()]
        public void OnTerrainChangedTest_ExcludedByPrediction()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.GetTerrain(0, 0).IsOwned;
            var after = !before;

            logic.OnTerrainChanged(this,
                new Event.TerrainChangedEventArgs(
                    logic.State.TurnID,
                     null,
                    new StateChanges.TerrainStateChange(new Location(0, 0), new Dictionary<string, object>() { { "IsOwned", after } })));

            Assert.Equal(before, logic.State.GetTerrain(0, 0).IsOwned);
        }

        [Fact()]
        public void OnGameStateChangedTest_Applied()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.CreditsPerCity;
            var after = before + 50;

            logic.OnGameStateChanged(this,
                new Event.GameStateChangedArgs(
                    logic.State.TurnID + 1,
                     null,
                    new StateChanges.GameStateChange(new Dictionary<string, object>() { { "CreditsPerCity", after } })));

            Assert.Equal(after, logic.State.CreditsPerCity);
        }

        [Fact()]
        public void OnGameStateChangedTest_ExcludedByPrediction()
        {
            var logic = GetLogicForEvents();

            var before = logic.State.CreditsPerCity;
            var after = before + 50;

            logic.OnGameStateChanged(this,
                new Event.GameStateChangedArgs(
                    logic.State.TurnID,
                     null,
                    new StateChanges.GameStateChange(new Dictionary<string, object>() { { "CreditsPerCity", after } })));

            Assert.Equal(before, logic.State.CreditsPerCity);
        }

        [Fact()]
        public void OnGameStartTest()
        {
            var fields = BattleGameStateTests.GetFields();
            fields = new BattleGameState.Fields(fields.Height + 1, fields.Width + 1, fields.Terrain, fields.Units, fields.Commanders, fields.Values, -1);
            var logic = GetLogicForEvents();

            logic.OnGameStart(this,
                new Event.GameStartEventArgs(fields));

            fields = new BattleGameState.Fields(fields.Height, fields.Width, fields.Terrain, fields.Units, fields.Commanders, fields.Values, 0);
            BattleGameStateTests.CrossCheckFieldsWithState(fields, logic.Logic.InternalState);
            Assert.Equal(0, logic.State.CurrentCommander.CommanderID);
        }

        [Fact()]
        public void OnUserAddedTest_Us()
        {
            var logic = GetLogicForEvents();

            Assert.True(logic.Logic.Users.ContainsKey(0));
            Assert.Equal(logic.User, logic.Logic.Users[0]);
        }

        [Fact()]
        public void OnUserAddedTest_NotUs()
        {
            var logic = GetLogicForEvents();

            var user = new User(1, "hi");
            logic.OnUserAdded(this,
                new Event.UserAddedEventArgs(user));

            Assert.True(logic.Users.ContainsKey(1));
            Assert.Equal(user, logic.Users[1]);
        }

        [Fact()]
        public void OnUserRemovedTest()
        {
            var logic = GetLogicForEvents();
            var user = new User(1, "hi");
            logic.OnUserAdded(this, new Event.UserAddedEventArgs(user));
            logic.OnUserRemoved(this, new Event.UserRemovedEventArgs(user.UserID));

            Assert.False(logic.Users.ContainsKey(user.UserID));
            Assert.False(logic.Users.Values.Contains(user));
        }

        [Fact()]
        public void OnUserAssignedToCommanderTest()
        {
            var logic = GetLogicForEvents();
            var user = new User(1, "hi");
            logic.OnUserAdded(this, new Event.UserAddedEventArgs(user));
            logic.OnUserAssignedToCommander(this, new Event.UserAssignedToCommanderEventArgs(user.UserID, 0, user.UserID, user.IsHost));

            Assert.True(logic.IsUserCommanding(user.UserID, 0));
            Assert.Equal(user.UserID, logic.CommanderAssignments[0]);
        }

        [Fact()]
        public void OnTurnChangedTest_TurnEnded_Applied()
        {
            var logic = GetLogicForEvents();
            logic.Logic.DoAction(new ActionInfo(ActionTypes.ForCommanders.EndTurn.Instance, new ActionContext(logic.State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, new CommanderContext(logic.State.CurrentCommander.CommanderID), new OtherContext())));
            var turnID = logic.State.TurnID;
            logic.OnTurnChanged(this, 
                new Event.TurnChangedEventArgs(turnID, null, new StateChanges.TurnChanged(turnID, turnID + 1, 0, 1, StateChanges.TurnChanged.Cause.TurnEnded)));

            Assert.Equal(turnID + 1, logic.State.TurnID);
        }

        [Fact()]
        public void OnTurnChangedTest_TurnEnded_ExcludedByPrediction()
        {
            var logic = GetLogicForEvents();
            var turnID = logic.State.TurnID;
            logic.OnTurnChanged(this,
                new Event.TurnChangedEventArgs(turnID, null, new StateChanges.TurnChanged(turnID, turnID + 1, 0, 1, StateChanges.TurnChanged.Cause.TurnEnded)));

            Assert.Equal(turnID, logic.State.TurnID);
        }

        [Fact()]
        public void OnTurnChangedTest_GameStart()
        {
            var logic = GetLogicForEvents();
            var turnID = logic.State.TurnID;
            logic.OnTurnChanged(this,
                new Event.TurnChangedEventArgs(turnID, null, new StateChanges.TurnChanged(-1, 0, -1, 0, StateChanges.TurnChanged.Cause.GameStart)));

            Assert.Equal(turnID, logic.State.TurnID);
        }

        [Fact()]
        public void OnSyncTest()
        {
            var logic = GetLogicForEvents();
            var fields = BattleGameStateTests.GetFields();
            fields = new BattleGameState.Fields(fields.Height + 1, fields.Width + 1, fields.Terrain, fields.Units, fields.Commanders, fields.Values, -1);

            logic.OnSync(this, new Event.SyncEventArgs(0, fields, logic.GetFields()));

            BattleGameStateTests.CrossCheckFieldsWithState(fields, logic.Logic.InternalState);
        }

        [Fact()]
        public void ClientInfoPacketRecievedTest()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            var user = new User(0, "name", true);
            logic.Logic.StartGame(BattleGameStateTests.GetFields());
            logic.ClientInfoPacketRecieved(user);

            Assert.Equal(user, logic.User);
        }

        [Fact]
        public void Client_MessageReceivedTest_Protocol()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            var wrapper = new FakeServerToClientWrapper();
            client.FakeReceive(wrapper);

            Assert.Equal(1, wrapper.RunCount);
        }

        [Fact]
        public void Client_MessageReceivedTest_Notify()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            var wrapper = new FakeNotifyWrapper();
            client.FakeReceive(wrapper);

            Assert.Equal(2, wrapper.NotifyCount);
        }

        [Fact]
        public void Client_MessageReceivedTest_Invalid()
        {
            var client = new FakeClient();
            var handler = GetIUserLogic();
            var logic = new NetworkedGameLogic(client, "name", handler);
            var wrapper = new FakeNotValidWrapper();

            Assert.Throws<ArgumentException>(() =>
            {
                client.FakeReceive(wrapper);
            });
        }

        public NetworkedGameLogic GetLogicForEvents()
        {
            var client = new FakeClient();

            var logic = new NetworkedGameLogic(client, "name", new List<IUserLogic>());

            var user = new User(0, "name", true);
            logic.ClientInfoPacketRecieved(user);
            logic.OnUserAdded(this, new Event.UserAddedEventArgs(user));
            logic.Logic.StartGame(BattleGameStateTests.GetFields());
            logic.OnUserAssignedToCommander(this, new Event.UserAssignedToCommanderEventArgs(0, 1, 0, true));

            return logic;
        }

        public class FakeServerToClientWrapper : Network.MessageWrappers.ServerToClientProtocolMessageWrapper
        {
            public int RunCount { get; private set; }
            public override void Run(IClientProtocolLogic logic)
            {
                RunCount++;
            }
        }

        public class FakeNotifyWrapper : Network.MessageWrappers.NotifyMessageWrapper
        {
            public int NotifyCount { get; private set; }

            public override void Notify(IUserLogic logic)
            {
                NotifyCount++;
            }
        }

        public class FakeNotValidWrapper : MessageWrapper
        {

        }

        public class FakeClient : IClient
        {
            public Queue<object> SentMessages { get; } = new Queue<object>();

            public bool IsDisconnected
            {
                get
                {
                    return false;
                }
            }

            public FakeClient() : base()
            {

            }

            public event EventHandler<MessageReceivedEventArgs> MessageReceived;
            public event EventHandler<DisconnectedEventArgs> Disconnected;
            public event EventHandler<ExceptionEventArgs> Exception;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task Listen()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                return;
            }

            public void Send(object obj)
            {
                SentMessages.Enqueue(obj);
            }

            public void FakeReceive(MessageWrapper obj)
            {
                MessageReceived?.Invoke(this, (new FakeMessageReceivedEventArgs(obj)));
            }

            public void Write(string message)
            {
                throw new NotImplementedException();
            }

            public void Disconnect()
            {
                throw new NotImplementedException();
            }
        }

        public List<FakeIUserLogic> GetIUserLogic(params FakeIUserLogic.Callback[] callbacks)
        {
            return new List<FakeIUserLogic>()
            {
                new FakeIUserLogic(callbacks)
            };
        }

        public class FakeMessageReceivedEventArgs : MessageReceivedEventArgs
        {
            public FakeMessageReceivedEventArgs(MessageWrapper wrapper) : base("")
            {
                _wrapper = wrapper;
            }

            private MessageWrapper _wrapper;
            public override MessageWrapper Message
            {
                get
                {
                    return _wrapper;
                }
            }
        }
    }
}