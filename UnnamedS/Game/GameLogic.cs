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
    [ContractClass(typeof(ContractClassForGameLogic))]
    public abstract class GameLogic
    {

        public abstract IReadOnlyBattleGameState State { get; }

        public abstract IReadOnlyDictionary<int, int?> CommanderAssignments { get; }

        public abstract IReadOnlyDictionary<int, User> Users { get; }

        public int LastSyncID { get; protected set; } = 0;

        public abstract void AddUser(User user, IReadOnlyList<IUserLogic> logic);

        public abstract void RemoveUser(int userID);

        //public abstract void AssignUserToCommander(int? userID, int CommanderID, bool isHost);

        public abstract bool IsUserCommanding(int userID, int commanderID);

        public abstract void DoActions(List<ActionInfo> actions);

        public abstract void Sync(int syncID);

        public abstract void Sync(int syncID, BattleGameState.Fields fields, Fields logicFields);

        public abstract void StartGame(BattleGameState.Fields fields, BattleGameState.StartMode startMode);

        public virtual Fields GetFields()
        {
            return new Fields(Users.Values.ToArray(), CommanderAssignments.ToDictionary(kp => kp.Key, kp => kp.Value));
        }

        protected class UserSet
        {
            public IReadOnlyList<IUserLogic> Logic { get; }
            public User State { get; }

            public UserSet(IReadOnlyList<IUserLogic> logic, User state)
            {
                Contract.Requires<ArgumentNullException>(null != logic);
                Contract.Requires<ArgumentNullException>(null != state);
                Logic = logic;
                State = state;
            }

            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(null != Logic);
                Contract.Invariant(null != State);
            }
        }

        public class Fields
        {
            public User[] Users { get; }
            public Dictionary<int, int?> UserCommanderAssignments { get; }

            public Fields(User[] users, Dictionary<int, int?> userCommanderAssignments)
            {
                Users = users;
                UserCommanderAssignments = userCommanderAssignments;
            }
        }

    }

    [ContractClassFor(typeof(GameLogic))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForGameLogic : GameLogic
    {
        public override void AddUser(User user, IReadOnlyList<IUserLogic> logic)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentNullException>(null != logic);
        }

        public override void StartGame(BattleGameState.Fields fields, BattleGameState.StartMode startMode)
        {
            Contract.Requires<ArgumentNullException>(null != fields);
        }

        public override void Sync(int syncID, BattleGameState.Fields fields, Fields logicFields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);
            Contract.Requires<ArgumentNullException>(null != logicFields);
        }
    }
}
