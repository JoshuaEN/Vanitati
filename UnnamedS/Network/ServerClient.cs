using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Network.Protocol;

namespace UnnamedStrategyGame.Network
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ServerClient : Client, IUserLogic, IServerProtocolLogic
    {
        private readonly QueuedLock serverLock;
        private volatile LocalGameLogic _gameLogic;
        public LocalGameLogic GameLogic
        {
            get { return _gameLogic; }
            set { _gameLogic = value; }
        }
        public User User { get; }

        private bool clientIdentificationReceivedFlag = false;

        public ServerClient(TcpClient tcpClient, LocalGameLogic gameLogic, User user, QueuedLock serverLock) : base(tcpClient)
        {
            Contract.Requires<ArgumentNullException>(null != tcpClient);
            Contract.Requires<ArgumentNullException>(null != gameLogic);
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentNullException>(null != serverLock);

            this.serverLock = serverLock;
            GameLogic = gameLogic;
            User = user;
            MessageReceived += ServerClient_MessageReceived;
        }

        private void ServerClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(null != sender);
            Contract.Requires<ArgumentNullException>(null != e);

            var msg = e.Message;

            if(clientIdentificationReceivedFlag != true && msg is MessageWrappers.ClientHelloProtocolWrapper == false)
            {
                NotifyOfException(new Exceptions.InvalidMessageOrderException(
                        string.Format(
                            "{0} message must be received before ANY {1} messages",
                            typeof(MessageWrappers.ClientHelloProtocolWrapper).Name,
                            typeof(MessageWrappers.MessageWrapper).Name
                        )
                   )
                );
                return;
            }

            if (msg is MessageWrappers.ClientToServerProtocolMessageWrapper)
            {
                try
                {
                    serverLock.Enter();
                    (msg as MessageWrappers.ClientToServerProtocolMessageWrapper).Run(this);
                }
                finally
                {
                    serverLock.Exit();
                }
            }
            else if (msg is MessageWrappers.CallMessageWrapper)
            {
                if (msg is MessageWrappers.AuthInterfaces.ICommanderAuth &&
                    (msg as MessageWrappers.AuthInterfaces.ICommanderAuth).CommanderAuthCheck(GameLogic, User) != true)
                {
                    NotifyOfException(new Exceptions.IllegalCallAttempt($"User {User.UserID} claimed to be in command of commander they were not"));
                    return;
                }

                if(msg is MessageWrappers.AuthInterfaces.IUserAuth &&
                    (msg as MessageWrappers.AuthInterfaces.IUserAuth).UserIDForAuth != User.UserID)
                {
                    NotifyOfException(new Exceptions.IllegalCallAttempt($"User {User.UserID} claimed to be another user"));
                    return;
                }

                if((msg as MessageWrappers.CallMessageWrapper).RequiresHost == true && User.IsHost == false)
                {
                    NotifyOfException(new Exceptions.IllegalCallAttempt("You are not the host!"));
                    return;
                }

                try
                {
                    serverLock.Enter();
                    (msg as MessageWrappers.CallMessageWrapper).Call(GameLogic);
                }
                finally
                {
                    serverLock.Exit();
                }
            }
            else
            {
                throw new ArgumentException("Unacceptable Type Received of " + msg.GetType());
            }
        }

        private void NotifyOfException(Exception ex)
        {
            Send(new MessageWrappers.OnExceptionNotifyWrapper(new Game.Event.ExceptionEventArgs(ex)));
        }

        protected override void Disconnect(Exception disconnectCause)
        {
            try
            {
                if (User != null)
                    GameLogic.RemoveUser(User.UserID);
            }
            finally
            {
                base.Disconnect(disconnectCause);
            }
        }

        #region Event Handlers

        #region IUserLogic Handlers

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            Send(new MessageWrappers.OnActionsTakenNotifyWrapper(e));
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
            Send(new MessageWrappers.OnCommanderChangedNotifyWrapper(e));
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            Send(new MessageWrappers.OnUnitChangedNotifyWrapper(e));
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            Send(new MessageWrappers.OnTerrainChangedNotifyWrapper(e));
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            Send(new MessageWrappers.OnGameStateChangedNotifyWrapper(e));
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            Send(new MessageWrappers.OnGameStartNotifyWrapper(e));
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            Send(new MessageWrappers.OnUserAddedNotifyWrapper(e));
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            Send(new MessageWrappers.OnUserRemovedNotifyWrapper(e));
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            Send(new MessageWrappers.OnUserAssignedToCommanderNotifyWrapper(e));
        }

        public void OnTurnChanged(object sender, TurnChangedEventArgs e)
        {
            Send(new MessageWrappers.OnTurnChangedNotifyWrapper(e));
        }

        public void OnSync(object sender, SyncEventArgs e)
        {
            Send(new MessageWrappers.OnSyncNotifyWrapper(e));
        }

        public void OnException(object sender, Game.Event.ExceptionEventArgs e)
        {
            Send(new MessageWrappers.OnExceptionNotifyWrapper(e));
        }

        public void OnVictoryConditionAchieved(object sender, VictoryConditionAchievedEventArgs args)
        {
            Send(new MessageWrappers.OnVictoryConditionAchievedNotifyWrapper(args));
        }

        #endregion

        #region IServerProtocolLogic Handers

        public void ClientHelloReceived(ClientHelloData clientIdentification)
        {
            if (clientIdentificationReceivedFlag != false)
            {
                throw new Exceptions.InvalidMessageOrderException("Client Identification message already received");
            }
            clientIdentificationReceivedFlag = true;
            User.Name = clientIdentification.Name;
            Send(new MessageWrappers.ClientInfoPacketProtocolWrapper(User));
            GameLogic.AddUser(User, new List<IUserLogic>() { this });
            Send(new MessageWrappers.OnSyncNotifyWrapper(new SyncEventArgs(clientIdentification.InitialSyncID, GameLogic.State.GetFields(), GameLogic.GetFields())));
        }

        #endregion

        #endregion

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != serverLock);
            Contract.Invariant(null != GameLogic);
            Contract.Invariant(null != User);
        }
    }
}
