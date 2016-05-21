﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainAction : TileAction
    {
        public abstract ActionTriggers Triggers { get; }

        public sealed override bool CanUserTrigger
        {
            get { return Triggers.HasFlag(ActionTriggers.ManuallyByUser); }
        }

        public sealed override Category ActionCategory
        {
            get { return Category.Terrain; }
        }

        protected TerrainAction(string key) : base("terrain_" + key) { }

        [Flags]
        public enum ActionTriggers
        {
            ManuallyByUser = 1,
            DirectlyByGameLogic = 2,
            OnAnyTurnStart = 4,
            OnAnyTurnEnd = 8,
            OnOwnerTurnStart = 16,
            OnOwnerTurnEnd = 32,
            OnOccupyingUnitTurnStart = 64,
            OnOccupyingUnitTurnEnd = 128,
            None = 0         
        }

    }
}
