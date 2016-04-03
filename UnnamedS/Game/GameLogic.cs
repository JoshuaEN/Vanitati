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

        public abstract void RemovePlayer(int playerID);

        public virtual void DoActions(int playerID, List<ActionInfo> actions)
        {
            foreach(var action in actions)
            {
                DoAction(playerID, action);
            }
        }

        public abstract void DoAction(int playerID, ActionInfo action);

        public virtual void DoAction(int playerID, Location source, Location target, ActionType action)
        {
            DoAction(playerID, new ActionInfo(action, source, target));
        }

        public virtual void DoAction(int playerID, Location source, ActionType action)
        {
            DoAction(playerID, source, source, action);
        }

        public abstract void StartGame(int height, int width, Terrain[] terrain, Unit[] units, Player[] players, Dictionary<string, object> gameStateAttributes);

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
