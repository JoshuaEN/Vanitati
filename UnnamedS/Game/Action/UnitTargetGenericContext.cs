﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    public class UnitTargetGenericContext<T> : TileTargetGenericContext<UnitContext, T, ActionTypes.UnitAction.ActionTriggers>
    {
        public UnitTargetGenericContext(IReadOnlyBattleGameState state, ActionContext context, Load load = Load.Source | Load.Target) : base(state, context, load) { }
    }
}
