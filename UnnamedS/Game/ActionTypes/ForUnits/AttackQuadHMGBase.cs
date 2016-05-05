using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public abstract class AttackQuadHMGBase : AttackHeavyMachineGun
    {
        public override double DamagePerSubunit { get; } = 5;
        public override int BaseAccuracy { get; } = 50;

        protected AttackQuadHMGBase(string key) : base(key) { }
    }
}
