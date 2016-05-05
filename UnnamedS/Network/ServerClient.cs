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
        private readonly object serverLock;
        public LocalGameLogic GameLogic { get; }
        public User User { get; }

        private bool clientIdentificationReceivedFlag = false;

        public ServerClient(NetworkStream networkStream, LocalGameLogic gameLogic, User user, object serverLock) : base(networkStream)
        {
            Contract.Requires<ArgumentNullException>(null != networkStream);
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
                throw new Exceptions.InvalidMessageOrderException(
                        string.Format(
                            "{0} message must be received before ANY {1} messages",
                            typeof(MessageWrappers.ClientHelloProtocolWrapper).Name,
                            typeof(MessageWrappers.MessageWrapper).Name
                        )
                   );
            }

            if (msg is MessageWrappers.ClientToServerProtocolMessageWrapper)
            {
                lock(serverLock)
                {
                    (msg as MessageWrappers.ClientToServerProtocolMessageWrapper).Run(this);
                }
            }
            else if (msg is MessageWrappers.CallMessageWrapper)
            {
                if (msg is MessageWrappers.CommanderTypeCallWrapper &&
                    (msg as MessageWrappers.CommanderTypeCallWrapper).AuthCheck(GameLogic, User) != true)
                {
                    throw new Exceptions.IllegalCallAttempt();
                }

                lock(serverLock)
                {
                    (msg as MessageWrappers.CallMessageWrapper).Call(GameLogic);
                }
            }
            else
            {
                throw new ArgumentException("Unacceptable Type Received of " + msg.GetType());
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
            Send(new MessageWrappers.OnSyncNotifyWrapper(new SyncEventArgs(clientIdentification.InitialSyncID, GameLogic.State.GetFields())));
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
