using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public class NetworkedGameLogic : GameLogic, IPlayerLogic, Network.IClientProtocolLogic
    {
        public Network.Client Client { get; }

        private uint _playerId;
        public uint PlayerId
        {
            get { return _playerId; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(true == PlayerIdKnown, "Player ID has already been set");
                _playerId = value;
                PlayerIdKnown = true;
            }
        }
        public bool PlayerIdKnown { get; protected set; }
        private Task clientListern;
        private IReadOnlyList<IPlayerLogic> thisPlayerLogic;

        public NetworkedGameLogic(Network.Client client, IReadOnlyList<IPlayerLogic> logic) : base()
        {
            thisPlayerLogic = logic;
            Client = client;
            Client.Disconnected += Client_Disconnected;
            Client.Exception += Client_Exception;
            Client.MessageReceived += Client_MessageReceived;
            clientListern = Client.Listen();
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
                if(PlayerIdKnown == false)
                {
                    throw new Network.Exceptions.InvalidMessageOrderException(
                        String.Format(
                            "{0} message must be recieved before ANY {1} messages", 
                            typeof(Network.MessageWrappers.ClientInfoPacketProtocolWrapper).Name, 
                            typeof(Network.MessageWrappers.NotifyMessageWrapper).Name
                        )
                   );
                }

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

        public override void DoActions(uint playerId, List<ActionInfo> actions)
        {
            Client.Send(new Network.MessageWrappers.DoActionsCallWrapper(actions));
        }

        public readonly Queue<IReadOnlyList<IPlayerLogic>> pendingPlayerList = new Queue<IReadOnlyList<IPlayerLogic>>();

        public override void AddPlayer(IReadOnlyList<IPlayerLogic> logic)
        {
            throw new InvalidOperationException("Not supported for remote games");
            pendingPlayerList.Enqueue(logic);
        }

        public override void RemovePlayer(uint id)
        {
            throw new NotImplementedException();
        }

        public void ClientInfoPacketRecieved(Network.Protocol.ClientInfo clientInfo)
        {
            PlayerId = clientInfo.PlayerId;
        }

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            // TODO Add Action Verification code to cross-check that we agree with the server's result of the action, maybe?
        }

        public void OnPlayerChanged(object sender, PlayerChangedEventArgs e)
        {
            State.UpdatePlayer(e.ChangeInfo);
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            State.UpdateUnit(e.ChangeInfo);
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            State.UpdateTerrain(e.ChangeInfo);
        }

        public void OnGameStateChanged(object sender, OnGameStateChangedArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnThisPlayerAdded(object sender, OnThisPlayerAddedArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
