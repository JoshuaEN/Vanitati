using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public abstract class Land : TerrainType
    {
        public override TerrainClassifications Classification { get { return TerrainClassifications.Land; } }

        public override bool CanBePillage { get; } = false;

        protected Land(string key) : base(key) { }
    }
}
