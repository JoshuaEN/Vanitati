using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackQuadHMGVsGround : AttackQuadHMGBase
    {

        private AttackQuadHMGVsGround() : base("quad_hmg_vs_ground") { }
        new public static AttackQuadHMGVsGround Instance { get; } = new AttackQuadHMGVsGround();
    }
}
