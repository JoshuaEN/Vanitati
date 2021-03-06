﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public abstract class LandStructure : Land
    {
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Industrialized;
        public override TerrainHeight Height { get; } = TerrainHeight.Normal;

        public override bool CanCapture { get; } = true;
        public override bool CanBePillage { get; } = true;
        public override bool CanRepair { get; } = true;
        public override bool CanSupply { get; } = true;

        public override int MaxHealth { get; } = 50;

        protected LandStructure(string key) : base(key) { }
    }
}
