using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    /// <summary>
    /// Read only interface for BattleGameState
    /// </summary>
    [ContractClass(typeof(ContractClassForIReadOnlyBattleGameState))]
    public interface IReadOnlyBattleGameState : Properties.IPropertyContainer
    {
        int Height { get; }
        int Width { get; }

        IReadOnlyDictionary<int, Unit> Units { get; }

        IReadOnlyCollection<Terrain> Terrain { get; }

        IReadOnlyDictionary<int, Commander> Commanders { get; }

        int TurnID { get; }
        int CreditsPerCity { get; }

        Commander CurrentCommander { get; }

        int GetNextUnitID();

        Unit GetUnit(Location loc);
        Unit GetUnit(int x, int y);
        Unit GetUnit(int unitId);

        Terrain GetTerrain(Location loc);
        Terrain GetTerrain(int x, int y);

        Tile GetTile(Location loc);
        Tile GetTile(int x, int y);

        Commander GetCommander(int commanderID);
        Commander SafeGetCommander(int commanderID);

        bool LocationsAdjacent(Location a, Location b);

        List<Location> LocationsAroundPoint(Location point, int minimum, int maximum);
        List<Location> LocationsAroundPoint(Location point, int range);

        BattleGameState Fork();

        BattleGameState.Fields GetFields();

    }

    [ContractClassFor(typeof(IReadOnlyBattleGameState))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForIReadOnlyBattleGameState : IReadOnlyBattleGameState
    {
        public abstract IReadOnlyDictionary<int, Commander> Commanders { get; }
        public abstract int CreditsPerCity { get; }
        public abstract Commander CurrentCommander { get; }
        public abstract int Height { get; }
        public abstract IReadOnlyCollection<Terrain> Terrain { get; }
        public abstract int TurnID { get; }
        public abstract IReadOnlyDictionary<int, Unit> Units { get; }
        public abstract int Width { get; }

        public abstract BattleGameState Fork();
        public abstract Commander GetCommander(int commanderID);

        public BattleGameState.Fields GetFields()
        {
            Contract.Ensures(Contract.Result<BattleGameState.Fields>() != null);
            return null;
        }

        public abstract int GetNextUnitID();
        public abstract IDictionary<string, object> GetProperties();
        public abstract Terrain GetTerrain(Location loc);
        public abstract Terrain GetTerrain(int x, int y);
        public abstract Tile GetTile(Location loc);
        public abstract Tile GetTile(int x, int y);
        public abstract Unit GetUnit(int unitId);
        public abstract Unit GetUnit(Location loc);
        public abstract Unit GetUnit(int x, int y);
        public abstract IDictionary<string, object> GetWriteableProperties();

        public bool LocationsAdjacent(Location a, Location b)
        {
            Contract.Requires<ArgumentNullException>(null != a);
            Contract.Requires<ArgumentNullException>(null != b);
            return false;
        }


        public List<Location> LocationsAroundPoint(Location point, int range)
        {
            Contract.Requires<ArgumentNullException>(null != point);
            Contract.Requires<ArgumentException>(range >= 0);
            Contract.Ensures(Contract.Result<List<Location>>() != null);
            return null;
        }

        public List<Location> LocationsAroundPoint(Location point, int minimum, int maximum)
        {
            Contract.Requires<ArgumentNullException>(null != point);
            Contract.Requires<ArgumentException>(minimum <= maximum);
            Contract.Ensures(Contract.Result<List<Location>>() != null);
            return null;
        }

        public Commander SafeGetCommander(int commanderID)
        {
            throw new NotImplementedException();
        }

        public abstract void SetProperties(IDictionary<string, object> values);
    }
}
