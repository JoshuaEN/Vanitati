using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public class NetworkedGameLogic : GameLogic, IUserLogic, Network.IClientProtocolLogic
    {
        #region Properties

        public Network.Client Client { get; }

        public LocalGameLogic Logic { get; } = new LocalGameLogic();

        public override IReadOnlyBattleGameState State
        {
            get { return Logic.State; }
        }

        public override IReadOnlyDictionary<int, int?> CommanderAssignments
        {
            get { return Logic.CommanderAssignments; }
        }

        protected BattleGameState InternalState
        {
            get { return Logic.InternalState; }
        }

        private User _user;
        public User User
        {
            get { return _user; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(null == User, "User has already been set");
                _user = value;
            }
        }

        private Task clientListener;
        private IReadOnlyList<IUserLogic> thisPlayerLogic;

        #endregion

        #region Events

        public event EventHandler<UserSetEventArgs> UserSetByServer;
        protected void OnUserSetByServer()
        {
            var handler = UserSetByServer;
            if(handler != null)
            {
                handler(this, new UserSetEventArgs(User));
            }
        }

        #endregion

        public NetworkedGameLogic(System.Net.IPEndPoint remoteEP, string name, IReadOnlyList<IUserLogic> logic) : this(new Network.Client(remoteEP), name, logic)
        {
            Contract.Requires<ArgumentNullException>(null != remoteEP);
        }

        public NetworkedGameLogic(Network.Client client, string name, IReadOnlyList<IUserLogic> logic) : base()
        {
            Contract.Requires<ArgumentNullException>(null != client);

            thisPlayerLogic = logic;
            Client = client;
            Client.Disconnected += Client_Disconnected;
            Client.Exception += Client_Exception;
            Client.MessageReceived += Client_MessageReceived;
            clientListener = Client.Listen();
            Client.Send(new Network.MessageWrappers.ClientHelloProtocolWrapper(new Network.Protocol.ClientHelloData(name, ++LastSyncID)));
        }

        private void Client_MessageReceived(object sender, Network.MessageReceivedEventArgs e)
        {
            var m = e.Message;

            if(m is Network.MessageWrappers.ServerToClientProtocolMessageWrapper)
            {
                (m as Network.MessageWrappers.ServerToClientProtocolMessageWrapper).Run(this);
            }
            else if(m is Network.MessageWrappers.NotifyMessageWrapper)
            {
                //if(User == null)
                //{
                //    throw new Network.Exceptions.InvalidMessageOrderException(
                //        String.Format(
                //            "{0} message must be received before ANY {1} messages", 
                //            typeof(Network.MessageWrappers.ClientInfoPacketProtocolWrapper).Name, 
                //            typeof(Network.MessageWrappers.NotifyMessageWrapper).Name
                //        )
                //   );
                //}

                var notifier = (m as Network.MessageWrappers.NotifyMessageWrapper);

                notifier.Notify(this);
                NotifyPlayer(notifier);
            }
            else
            {
                throw new ArgumentException("Unacceptable Wrapper Type Received of " + m.GetType());
            }
        }

        public virtual void NotifyPlayer(Network.MessageWrappers.NotifyMessageWrapper notifyMessage)
        {
            foreach(var logic in thisPlayerLogic)
            {
                notifyMessage.Notify(logic);
            }
        }

        private void Client_Exception(object sender, Network.ExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Client_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void DoActions(List<ActionInfo> actions)
        {
            Client.Send(new Network.MessageWrappers.DoActionsCallWrapper(actions));
        }

        public override void DoAction(ActionInfo action)
        {
            DoActions(new List<ActionInfo>() { action });
        }

        public override void Sync(int syncID)
        {
            LastSyncID = syncID;
            Client.Send(new Network.MessageWrappers.SyncCallWrapper(syncID));
        }

        public override void StartGame(BattleGameState.Fields fields)
        {
            Client.Send(new Network.MessageWrappers.StartGameCallWrapper(fields));
        }

        public override void EndTurn(int commanderID)
        {
            Client.Send(new Network.MessageWrappers.EndTurnCallWrapper(commanderID));
        }

        public override void AddUser(User user, IReadOnlyList<IUserLogic> logic)
        {
            throw new NotSupportedException();
        }

        public override void RemoveUser(int userID)
        {
            Client.Send(new Network.MessageWrappers.RemoveUserCallWrapper(userID));
        }

        public override void AssignUserToCommander(int? userID, int commanderID)
        {
            Client.Send(new Network.MessageWrappers.AssignUserToCommanderCallWrapper(userID, commanderID));
        }

        #region Event Handlers

        public void ClientInfoPacketRecieved(User user)
        {
            User = user;

            OnUserSetByServer();
        }

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            // TODO Add Action Verification code to cross-check that we agree with the server's result of the action, maybe?
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
            InternalState.UpdateCommander(e.ChangeInfo);
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            InternalState.UpdateUnit(e.ChangeInfo);
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            InternalState.UpdateTerrain(e.ChangeInfo);
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            InternalState.SetProperties(e.ChangeInfo.UpdatedProperties);
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            Logic.StartGame(e.ChangeInfo);
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            Logic.AddUser(e.User, new List<IUserLogic>(0));   
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            Logic.RemoveUser(e.UserID);
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            Logic.AssignUserToCommander(e.UserID, e.CommanderID);
        }

        public void OnTurnEnded(object sender, TurnEndedEventArgs e)
        {
            Logic.EndTurn(e.CommanderID);
        }

        public void OnSync(object sender, SyncEventArgs e)
        {
            Logic.Sync(e.SyncID, e.Fields);
        }

        #endregion

        public class UserSetEventArgs : EventArgs
        {
            public User User { get; }

            public UserSetEventArgs(User user)
            {
                User = user;
            }
        }
    }
}
