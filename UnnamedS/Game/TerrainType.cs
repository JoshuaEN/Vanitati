using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public abstract class TerrainType : BaseType
    {
        public IReadOnlyList<UnitType> Buildable { get; }
        public bool Captureable { get; } = false;
        public int MaxCapturePoints { get; } = 10;

        protected TerrainType(
            string key, 
            Dictionary<string, object> attributeDefaults, 
            List<UnitType> buildable = null,
            bool captureable = false,
            int maxCapturePoints = 10) : base("terrain_" + key)
        {
            Buildable = buildable ?? new List<UnitType>(0);
        }

        public static IReadOnlyDictionary<string, TerrainType> TYPES { get; }
        static TerrainType()
        {
            TYPES = BuildTypeListing<TerrainType>("UnnamedStrategyGame.Game.TerrainTypes");
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
