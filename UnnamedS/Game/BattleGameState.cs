using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Game
{
    public class BattleGameState : GameState, IAttributeContainer
    {
        public virtual uint CurrentPlayer { get; protected set; }
        public virtual int TurnCounter { get; protected set; } = 1;

        private AttributeContainer attributeContainer { get; }

        protected Dictionary<int, Unit> _units { get; set; }

        /// <summary>
        /// Listing of all units on the map, the int being the index of the tile they are on.
        /// </summary>
        public IReadOnlyDictionary<int, Unit> Units
        {
            get { return _units; }
        }

        protected Terrain[] _terrain { get; set; }

        public IReadOnlyCollection<Terrain> Terrain
        {
            get
            {
                return Array.AsReadOnly(_terrain);
            }
        }

        protected Dictionary<uint, Player> _players { get; set; }

        public IReadOnlyDictionary<uint, Player> Players
        {
            get { return _players; }
        }

        public IReadOnlyList<IAttribute> Attributes
        {
            get
            {
                return ((IAttributeContainer)attributeContainer).Attributes;
            }
        }

        public override Unit GetUnit(int x, int y)
        {
            Unit unit;
            if(_units.TryGetValue(GetIndex(x, y), out unit) == false)
            {
                return null;
            }
            return unit;
        }

        public override Terrain GetTerrain(int x, int y)
        {
            var idx = GetIndex(x, y);

            if(idx >= _terrain.Length)
            {
                return null;
            }
            else
            {
                return _terrain[idx];
            }
        }

        public override void SetUnit(int x, int y, Unit value)
        {
            _units[GetIndex(x, y)] = value;
        }

        public override void SetTerrain(int x, int y, Terrain value)
        {
            _terrain[GetIndex(x, y)] = value;
        }

        private int GetIndex(int x, int y)
        {
            Contract.Requires<ArgumentException>(0 >= x, "X Cord should be >= 0");
            Contract.Requires<ArgumentException>(0 >= y, "Y Cord should be >= 0");
            Contract.Ensures(Contract.Result<int>() >= 0, "Resulting index should be >= 0");
            Contract.Ensures(Contract.Result<int>() <= _terrain.Length, "Resulting index should be <= game board size");

            var i = x * Width + y;

            if(i >= _terrain.Length)
            {
                throw new IndexOutOfRangeException("X Y Coordinate outside of valid range.");
            }

            return i;
        }

        public override void DeleteUnit(int x, int y)
        {
            _units.Remove(GetIndex(x, y));
        }

        protected uint nextPlayerId = Game.Globals.UNIQUE_PLAYER_ID_RESERVED_NAMESPACE_END;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<AttributeChangedEventArgs> AttributeChanged;

        public uint AddPlayer()
        {
            var id = nextPlayerId++;
            _players.Add(id, new Player(id));
            return id;
        }

        public void RemovePlayer(uint id)
        {
            _players.Remove(id);
        }

        public Player GetPlayer(uint id)
        {
            Player p;
            if(_players.TryGetValue(id, out p) == false)
            {
                throw new Exceptions.UnknownPlayerException();
            }

            return p;
        }

        public virtual void EndCurrentTurn()
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayer(PlayerStateChange changeInfo)
        {
            GetPlayer(changeInfo.PlayerId).SetAttributes(changeInfo.UpdatedAttributes);
        }

        public void UpdateUnit(UnitStateChange changeInfo)
        {
            GetUnit(changeInfo.ChangedUnitLocation).SetAttributes(changeInfo.UpdatedAttributes);
        }

        public void UpdateTerrain(TerrainStateChange changeInfo)
        {
            GetTerrain(changeInfo.ChangedTerrainLocation).SetAttributes(changeInfo.UpdatedAttributes);
        }

        public IAttribute GetAttribute(string key)
        {
            return ((IAttributeContainer)attributeContainer).GetAttribute(key);
        }

        public void SetAttribute(IAttribute value)
        {
            ((IAttributeContainer)attributeContainer).SetAttribute(value);
        }

        public void SetAttributeReadOnly(string key)
        {
            ((IAttributeContainer)attributeContainer).SetAttributeReadOnly(key);
        }

        public void SetAttributes(IReadOnlyList<IAttribute> values)
        {
            ((IAttributeContainer)attributeContainer).SetAttributes(values);
        }
    }
}
