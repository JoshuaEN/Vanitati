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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Terrain : IPropertyContainer
    {
        public TerrainType TerrainType { get; private set; }
        public Location Location { get; }
        public Dictionary<int, int> CaptureProgress { get; private set; }
        public bool IsOwned { get; private set; }
        public int CommanderID { get; private set; }
        public int Health { get; private set; }
        public int DigIn { get; private set; }

        public Terrain(string terrainTypeKey, Location location, Dictionary<int, int> captureProgress = null) : this(TerrainType.TYPES[terrainTypeKey], location, captureProgress)
        {
            Contract.Requires<ArgumentNullException>(terrainTypeKey != null);
            Contract.Requires<ArgumentNullException>(null != location);
        }

        [Newtonsoft.Json.JsonConstructor]
        public Terrain(TerrainType terrainType, Location location, Dictionary<int, int> captureProgress = null, bool isOwned = false, int commanderID = -1, int? health = null, int? digIn = null)
        {
            Contract.Requires<ArgumentNullException>(terrainType != null);
            Contract.Requires<ArgumentNullException>(location != null);

            TerrainType = terrainType;
            Location = location;
            CaptureProgress = captureProgress ?? new Dictionary<int, int>();
            IsOwned = isOwned;
            CommanderID = commanderID;
            Health = health ?? TerrainType.MaxHealth;
            DigIn = digIn ?? 0;
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

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != TerrainType);
            Contract.Invariant(null != Location);
            Contract.Invariant(null != CaptureProgress);
        }
    }
}
