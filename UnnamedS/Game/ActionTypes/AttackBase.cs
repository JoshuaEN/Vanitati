using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    public abstract class AttackBase : ActionType
    {
        protected AttackBase(string key) : base("attack_" + key, ActionTarget.AnyOtherUnit, ActionTriggers.None, true) { }
    }
}
