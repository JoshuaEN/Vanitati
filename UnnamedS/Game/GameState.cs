using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForGameState))]
    public abstract class GameState
    {
        public virtual int Height { get; protected set; }
        public virtual int Width { get; protected set; }

        public virtual Unit GetUnit(Location loc)
        {
            return GetUnit(loc.X, loc.Y);
        }
        public abstract Unit GetUnit(int x, int y);
        public virtual Terrain GetTerrain(Location loc)
        {
            return GetTerrain(loc.X, loc.Y);
        }
        public abstract Terrain GetTerrain(int x, int y);
        public virtual Tile GetTile(Location loc)
        {
            return GetTile(loc.X, loc.Y);
        }
        public virtual Tile GetTile(int x, int y)
        {
            return new Tile(GetTerrain(x, y), GetUnit(x, y));
        }


        public virtual void SetUnit(Location loc, Unit value)
        {
            SetUnit(loc.X, loc.Y, value);
        }
        public abstract void SetUnit(int x, int y, Unit value);
        public virtual void SetTerrain(Location loc, Terrain value)
        {
            SetTerrain(loc.X, loc.Y, value);
        }
        public abstract void SetTerrain(int x, int y, Terrain value);
        public virtual void SetTile(Location loc, Tile value)
        {
            SetTile(loc.X, loc.Y, value);
        }
        public virtual void SetTile(int x, int y, Tile value)
        {
            SetUnit(x, y, value.Unit);
            SetTerrain(x, y, value.Terrain);
        }

        public virtual void DeleteUnit(Location loc)
        {
            DeleteUnit(loc.X, loc.Y);
        }
        public abstract void DeleteUnit(int x, int y);

    }

    [ContractClassFor(typeof(GameState))]
    internal abstract class ContractClassForGameState : GameState
    {
        public override Terrain GetTerrain(int x, int y)
        {
            Contract.Requires<ArgumentOutOfRangeException>(x > 0);
            Contract.Requires<ArgumentOutOfRangeException>(y > 0);
            return null;
        }

        public override Unit GetUnit(int x, int y)
        {
            Contract.Requires<ArgumentOutOfRangeException>(x > 0);
            Contract.Requires<ArgumentOutOfRangeException>(y > 0);
            return null;
        }

        public override void SetTerrain(int x, int y, Terrain value)
        {
            Contract.Requires<ArgumentOutOfRangeException>(x > 0);
            Contract.Requires<ArgumentOutOfRangeException>(y > 0);
            Contract.Requires<ArgumentNullException>(value != null);
        }

        public override void SetUnit(int x, int y, Unit value)
        {
            Contract.Requires<ArgumentOutOfRangeException>(x > 0);
            Contract.Requires<ArgumentOutOfRangeException>(y > 0);
            Contract.Requires<ArgumentNullException>(value != null);
        }
    }
}
