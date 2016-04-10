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

        public abstract IReadOnlyBattleGameState State { get; }

        public abstract IReadOnlyDictionary<int, int?> CommanderAssignments { get; }

        public int LastSyncID { get; protected set; } = 0;

        public abstract void AddUser(User user, IReadOnlyList<IUserLogic> logic);

        public abstract void RemoveUser(int userID);

        public abstract void AssignUserToCommander(int? userID, int CommanderID);

        public virtual void DoActions(List<ActionInfo> actions)
        {
            foreach(var action in actions)
            {
                DoAction(action);
            }
        }

        public abstract void DoAction(ActionInfo action);

        public virtual void Sync()
        {
            Sync(++LastSyncID);
        }

        public abstract void Sync(int syncID);

        public abstract void StartGame(BattleGameState.Fields fields);

        public abstract void EndTurn(int commanderID);

        protected class UserSet
        {
            public IReadOnlyList<IUserLogic> Logic { get; }
            public User State { get; }

            public UserSet(IReadOnlyList<IUserLogic> logic, User state)
            {
                Logic = logic;
                State = state;
            }
        }
    }
}
