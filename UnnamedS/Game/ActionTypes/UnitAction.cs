using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitAction : TileAction
    {
        public abstract ActionTriggers Triggers { get; }

        public virtual bool CausesMovement { get; }

        public sealed override bool CanUserTrigger
        {
            get { return Triggers.HasFlag(ActionTriggers.ManuallyByUser); }
        }

        public sealed override Category ActionCategory
        {
            get { return Category.Unit; }
        }

        protected UnitAction(string key) : base("unit_" + key) { }

        [Flags]
        public enum ActionTriggers {
            ManuallyByUser = 1,
            DirectlyByGameLogic = 2,
            TurnStart = 4,
            TurnEnd = 8,
            UnitCreated = 16,
            UnitDestroyed = 32,
            PropertyChanged = 64,
            ActionPerformedByUser = 128,
            AttackRetaliation = 256,
            None = 0
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(Triggers != ActionTriggers.PropertyChanged || (typeof(ForUnits.UnitTargetGenericAction<IDictionary<string, object>>).IsAssignableFrom(GetType())));
            Contract.Invariant(Triggers != ActionTriggers.ActionPerformedByUser || (typeof(ForUnits.UnitTargetGenericAction<ActionInfo>).IsAssignableFrom(GetType())));
        }

    }
}
