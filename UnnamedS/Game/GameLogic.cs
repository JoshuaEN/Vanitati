using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public abstract class GameLogic
    {
        private BattleGameState _state;
        public BattleGameState State
        {
            get
            {
                return _state;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(null != value);
                _state = value;
            }
        }

        public GameLogic(BattleGameState state)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            State = state;
        }

        protected GameLogic()
        {
            _state = null;
        }

        public abstract void AddPlayer(IReadOnlyList<IPlayerLogic> logic);

        public abstract void RemovePlayer(uint id);

        public abstract void DoActions(uint playerId, List<ActionInfo> actions);

        public virtual void DoAction(uint playerId, ActionInfo action)
        {
            DoActions(playerId, new List<ActionInfo>() { action });
        }

        public virtual void DoAction(uint playerId, Location source, Location target, ActionType action)
        {
            DoAction(playerId, new ActionInfo(action, source, target));
        }

        public virtual void DoAction(uint playerId, Location source, ActionType action)
        {
            DoAction(playerId, source, null, action);
        }


        protected class PlayerSet
        {
            public IReadOnlyList<IPlayerLogic> Logic { get; }
            public Player State { get; }

            public PlayerSet(IReadOnlyList<IPlayerLogic> logic, Player state)
            {
                Logic = logic;
                State = state;
            }
        }
    }
}
