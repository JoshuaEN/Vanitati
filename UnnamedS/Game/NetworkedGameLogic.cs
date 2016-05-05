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

        public Network.IClient Client { get; }

        public LocalGameLogic Logic { get; } = new LocalGameLogic();

        public override IReadOnlyBattleGameState State
        {
            get { return Logic.State; }
        }

        public override IReadOnlyDictionary<int, User> Users
        {
            get { return Logic.Users; }
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
        private readonly string chosenName;

        public bool Listening { get; private set; }

#if NETWORK_PREDICTION
        
        private bool CurrentlyUserTurn { get; set; }
        private int UserTurnID { get; set; }
#endif

        #endregion

        #region Events

        public event EventHandler<UserSetEventArgs> UserSetByServer;
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected void OnUserSetByServer()
        {
            var handler = UserSetByServer;
            if(handler != null)
            {
                handler(this, new UserSetEventArgs(User));
            }
        }

        public event EventHandler<Network.DisconnectedEventArgs> Disconnected;
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected void OnDisconnected(Network.DisconnectedEventArgs e)
        {
            var handler = Disconnected;
            if(null != handler)
            {
                handler(this, e);
            }
        }

        public event EventHandler<Network.ExceptionEventArgs> NetworkException;
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected void OnNetworkException(Network.ExceptionEventArgs e)
        {
            var handler = NetworkException;
            if(null != handler)
            {
                handler(this, e);
            }
        }

        #endregion
        // Untestable
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public NetworkedGameLogic(System.Net.IPEndPoint remoteEP, string name, IReadOnlyList<IUserLogic> logic) : this(new Network.Client(remoteEP), name, logic)
        {
            Contract.Requires<ArgumentNullException>(null != remoteEP);
            Contract.Requires<ArgumentNullException>(null != name);
            Contract.Requires<ArgumentNullException>(null != logic);
        }

        public NetworkedGameLogic(Network.IClient client, string name, IReadOnlyList<IUserLogic> logic) : base()
        {
            Contract.Requires<ArgumentNullException>(null != client);
            Contract.Requires<ArgumentNullException>(null != name);
            Contract.Requires<ArgumentNullException>(null != logic);

            thisPlayerLogic = logic;
            Client = client;
            chosenName = name;
            Client.Disconnected += Client_Disconnected;
            Client.Exception += Client_Exception;
            Client.MessageReceived += Client_MessageReceived;
        }

        public void Listen()
        {
            Contract.Requires<InvalidOperationException>(false == Listening, "Already listening");

            Listening = true;
            clientListener = Client.Listen();
            Client.Send(new Network.MessageWrappers.ClientHelloProtocolWrapper(new Network.Protocol.ClientHelloData(chosenName, ++LastSyncID)));
        }

        private void NotifyPlayer(Network.MessageWrappers.NotifyMessageWrapper notifyMessage)
        {
            foreach(var logic in thisPlayerLogic)
            {
                notifyMessage.Notify(logic);
            }
        }

        #region Game Logic Implementation

        public override void DoActions(List<ActionInfo> actions)
        {
#if NETWORK_PREDICTION
            Logic.DoActions(actions);
#endif
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

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override bool IsUserCommanding(int userID, int commanderID)
        {
            return Logic.IsUserCommanding(userID, commanderID);
        }

        #endregion

        #region Helper Methods

#if NETWORK_PREDICTION
        private void UpdateCurrentlyUserTurn()
        {
            if (User != null && State.CurrentCommander != null)
            {
                CurrentlyUserTurn = Logic.IsUserCommanding(User.UserID, State.CurrentCommander.CommanderID);
                UserTurnID = State.TurnID;
            }
            else
            {
                CurrentlyUserTurn = false;
                UserTurnID = -1;
            }
        }

        private bool ShouldUpdateStateInformation(ITurnAwareEvent e)
        {
            return CurrentlyUserTurn == false || UserTurnID != e.TurnID;
        }
#endif

        #endregion

        #region Event Handlers

        #region Network Client

        private void Client_MessageReceived(object sender, Network.MessageReceivedEventArgs e)
        {
            var m = e.Message;

            if (m is Network.MessageWrappers.ServerToClientProtocolMessageWrapper)
            {
                (m as Network.MessageWrappers.ServerToClientProtocolMessageWrapper).Run(this);
            }
            else if (m is Network.MessageWrappers.NotifyMessageWrapper)
            {
                var notifier = (m as Network.MessageWrappers.NotifyMessageWrapper);

                notifier.Notify(this);
                NotifyPlayer(notifier);
            }
            else
            {
                throw new ArgumentException("Unacceptable Wrapper Type Received of " + m.GetType());
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private void Client_Exception(object sender, Network.ExceptionEventArgs e)
        {
            OnNetworkException(e);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private void Client_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {
            OnDisconnected(e);
        }

        #endregion

        #region IUserLogic

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            // TODO Add Action Verification code to cross-check that we agree with the server's result of the action, maybe?
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
#if NETWORK_PREDICTION
            if(ShouldUpdateStateInformation(e))
#endif
                InternalState.UpdateCommander(e.ChangeInfo);
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
#if NETWORK_PREDICTION
            if (ShouldUpdateStateInformation(e))
#endif
                InternalState.UpdateUnit(e.ChangeInfo);
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
#if NETWORK_PREDICTION
            if (ShouldUpdateStateInformation(e))
#endif
                InternalState.UpdateTerrain(e.ChangeInfo);
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
#if NETWORK_PREDICTION
            if (ShouldUpdateStateInformation(e))
#endif
                InternalState.SetProperties(e.ChangeInfo.UpdatedProperties);
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            Logic.StartGame(e.ChangeInfo);
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            IReadOnlyList<IUserLogic> list;
            if (User != null && e.User.UserID == User.UserID)
                list = thisPlayerLogic;
            else
                list = new List<IUserLogic>(0);

            Logic.AddUser(e.User, list);   
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            Logic.RemoveUser(e.UserID);
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            Logic.AssignUserToCommander(e.UserID, e.CommanderID);
#if NETWORK_PREDICTION
            UpdateCurrentlyUserTurn();
#endif
        }

        public void OnTurnChanged(object sender, TurnChangedEventArgs e)
        {
            if(e.ChangeInfo.ChangeCause == StateChanges.TurnChanged.Cause.TurnEnded
#if NETWORK_PREDICTION
                && ShouldUpdateStateInformation(e)
#endif
                )
            {
                Logic.EndTurn(e.ChangeInfo.PreviousCommanderID);
            }
#if NETWORK_PREDICTION
            UpdateCurrentlyUserTurn();
#endif
        }

        public void OnSync(object sender, SyncEventArgs e)
        {
            Logic.Sync(e.SyncID, e.Fields);
#if NETWORK_PREDICTION
            UpdateCurrentlyUserTurn();
#endif
        }

#endregion

#region IClientProtocolLogic

        public void ClientInfoPacketRecieved(User user)
        {
            User = user;

            OnUserSetByServer();
        }

#endregion

#endregion

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Logic);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
