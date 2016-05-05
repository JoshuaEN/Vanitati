﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class OtherContext : Context
    {
        public override bool CanBeSource
        {
            get
            {
                return false;
            }
        }

        public override ActionType.Category ActionCategory
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override ActionType.TargetCategory ActionTargetCategory
        {
            get
            {
                return ActionType.TargetCategory.Other;
            }
        }
    }
}
