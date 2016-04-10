using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Network.Protocol;

namespace UnnamedStrategyGame.Network
{
    public class ServerClient : Client, IUserLogic, IServerProtocolLogic
    {
        public LocalGameLogic GameLogic { get; }
        public User User { get; }

        private bool clientIdentificationReceivedFlag = false;

        public ServerClient(NetworkStream networkStream, LocalGameLogic gameLogic, User user) : base(networkStream)
        {
            GameLogic = gameLogic;
            User = user;
            gameLogic.AddUser(user, new List<IUserLogic>() { this });
            MessageReceived += ServerClient_MessageReceived;
        }

        private void ServerClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
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
                (msg as MessageWrappers.ClientToServerProtocolMessageWrapper).Run(this);
            }
            else if (msg is MessageWrappers.CallMessageWrapper)
            {
                if (msg is MessageWrappers.CommanderTypeCallWrapper &&
                    (msg as MessageWrappers.CommanderTypeCallWrapper).AuthCheck(GameLogic, User) != true)
                {
                    throw new Exceptions.IllegalCallAttempt();
                }

                (msg as MessageWrappers.CallMessageWrapper).Call(GameLogic);
            }
            else
            {
                throw new ArgumentException("Unacceptable Type Received of " + msg.GetType());
            }
        }

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

        public void ClientHelloReceived(ClientHelloData clientIdentification)
        {
            if(clientIdentificationReceivedFlag != false)
            {
                throw new Exceptions.InvalidMessageOrderException("Client Identification message already received");
            }
            clientIdentificationReceivedFlag = true;
            User.Name = clientIdentification.Name;
            Send(new MessageWrappers.ClientInfoPacketProtocolWrapper(User));
            Send(new MessageWrappers.OnSyncNotifyWrapper(new SyncEventArgs(clientIdentification.InitialSyncID, GameLogic.State.GetFields())));
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

        public void OnTurnEnded(object sender, TurnEndedEventArgs e)
        {
            Send(new MessageWrappers.OnTurnEndedNotifyWrapper(e));
        }

        public void OnSync(object sender, SyncEventArgs e)
        {
            Send(new MessageWrappers.OnSyncNotifyWrapper(e));
        }
    }
}
