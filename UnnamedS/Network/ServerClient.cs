using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Network
{
    public class ServerClient : Client, IPlayerLogic
    {
        public LocalGameLogic GameLogic { get; }
        public int PlayerId { get; }
        public ServerClient(NetworkStream networkStream, LocalGameLogic gameLogic) : base(networkStream)
        {
            GameLogic = gameLogic;
            PlayerId = gameLogic.AddPlayerLocally(new List<IPlayerLogic>() { this });
            MessageReceived += ServerClient_MessageReceived;
            Send(new MessageWrappers.ClientInfoPacketProtocolWrapper(new Protocol.ClientInfo(PlayerId, true)));
        }

        private void ServerClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var msg = e.Message;

            if(msg is MessageWrappers.CallMessageWrapper)
            {
                (msg as MessageWrappers.CallMessageWrapper).Call(PlayerId, GameLogic);
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

        public void OnPlayerChanged(object sender, PlayerChangedEventArgs e)
        {
            Send(new MessageWrappers.OnPlayerChangedNotifyWrapper(e));
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

        public void OnThisPlayerAdded(object sender, ThisPlayerAddedArgs e)
        {
            Send(new MessageWrappers.OnThisPlayerAddedNotifyWrapper(e));
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            Send(new MessageWrappers.OnGameStartNotifyWrapper(e));
        }
    }
}
