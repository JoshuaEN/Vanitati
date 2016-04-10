using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;

namespace UnnamedStrategyGame.Game
{
    public class Terrain : IPropertyContainer
    {
        public TerrainType TerrainType { get; set; }
        public Location Location { get; set; }
        public Dictionary<int, int> CaptureProgress { get; set; }

        public Terrain(string terrainTypeKey, Location location) : this(TerrainType.TYPES[terrainTypeKey], location)
        {
            Contract.Requires<ArgumentNullException>(terrainTypeKey != null);
        }

        [Newtonsoft.Json.JsonConstructor]
        public Terrain(TerrainType terrainType, Location location)
        {
            Contract.Requires<ArgumentNullException>(terrainType != null);
            Contract.Requires<ArgumentNullException>(location != null);
            TerrainType = terrainType;
            Location = location;
        }

        public IDictionary<string, object> GetProperties()
        {
            return PropertyContainer.TERRAIN.GetProperties(this);
        }

        public IDictionary<string, object> GetWriteableProperties()
        {
            return PropertyContainer.TERRAIN.GetWriteableProperties(this);
        }

        public void SetProperties(IDictionary<string, object> values)
        {
            PropertyContainer.TERRAIN.SetProperties(this, values);
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}", TerrainType, Location);
        }
    }
}
