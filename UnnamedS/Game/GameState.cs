using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForGameState))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class GameState
    {
        [JsonIgnore]
        public virtual int Height { get; protected set; }
        [JsonIgnore]
        public virtual int Width { get; protected set; }

        [Pure]
        public virtual Unit GetUnit(Location loc)
        {
            Contract.Requires<ArgumentNullException>(null != loc);
            return GetUnit(loc.X, loc.Y);
        }

        [Pure]
        public abstract Unit GetUnit(int x, int y);
        [Pure]
        public virtual Terrain GetTerrain(Location loc)
        {
            Contract.Requires<ArgumentNullException>(null != loc);
            return GetTerrain(loc.X, loc.Y);
        }
        [Pure]
        public abstract Terrain GetTerrain(int x, int y);
        [Pure]
        public virtual Tile GetTile(Location loc)
        {
            Contract.Requires<ArgumentNullException>(null != loc);
            return GetTile(loc.X, loc.Y);
        }
        [Pure]
        public virtual Tile GetTile(int x, int y)
        {
            return new Tile(GetTerrain(x, y), GetUnit(x, y));
        }

        public abstract void AddUnit(Unit unit);

        public virtual void DeleteUnit(Location loc)
        {
            Contract.Requires<ArgumentNullException>(null != loc);
            DeleteUnit(loc.X, loc.Y);
        }
        public abstract void DeleteUnit(int x, int y);

    }

    [ContractClassFor(typeof(GameState))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForGameState : GameState
    {
        public override Terrain GetTerrain(int x, int y)
        {
            return null;
        }

        public override Unit GetUnit(int x, int y)
        {
            return null;
        }

        public override void AddUnit(Unit unit)
        {
            Contract.Requires<ArgumentNullException>(null != unit);
            Contract.Requires<ArgumentException>(GetUnit(unit.Location) == null);
        }
    }
}
