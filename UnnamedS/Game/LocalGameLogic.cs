using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public class LocalGameLogic : GameLogic
    {
        protected Dictionary<uint, PlayerSet> PlayerList { get; } = new Dictionary<uint, PlayerSet>();

        public LocalGameLogic(BattleGameState state) : base(state)
        {

        }

        public override void AddPlayer(IReadOnlyList<IPlayerLogic> logic)
        {
            var id = AddPlayerLocally(logic);
            var eventArgs = new Event.OnThisPlayerAddedArgs(id);
            foreach(var _logic in logic)
            {
                _logic.OnThisPlayerAdded(this, eventArgs);
            }
        }

        public uint AddPlayerLocally(IReadOnlyList<IPlayerLogic> logic)
        {
            var id = State.AddPlayer();
            PlayerList.Add(id, new PlayerSet(logic, new Player(id)));
            return id;
        }

        public override void RemovePlayer(uint id)
        {
            State.RemovePlayer(id);
            PlayerList.Remove(id);
        }

        public override void DoActions(uint playerId, List<ActionInfo> actions)
        {
            throw new NotImplementedException();
        }
    }
}
