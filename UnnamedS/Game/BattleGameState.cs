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
        //protected const string UNITS = "units";
        //protected const string TERRAIN = "terrain";
        //protected const string PLAYERS = "players";
        protected const string CURRENT_PLAYER = "current_player";
        protected const string TURN_COUNTER = "turn_counter";
        protected const string NEXT_PLAYER_ID = "next_player_id";
        protected const string NEXT_UNIT_ID = "next_unit_id";

        //protected static readonly AttributeDefinition<Dictionary<int, Unit>> UNITS_DEF = new AttributeDefinition<Dictionary<int, Unit>>(UNITS, new Dictionary<int, Unit>());
        //protected static readonly AttributeDefinition<Terrain[]> TERRAIN_DEF = new AttributeDefinition<Terrain[]>(TERRAIN, new Terrain[0]);
        //protected static readonly AttributeDefinition<Dictionary<int, Player>> PLAYERS_DEF = new AttributeDefinition<Dictionary<int, Player>>(PLAYERS, new Dictionary<int, Player>());
        protected static readonly AttributeDefinition<int> CURRENT_PLAYER_DEF = new AttributeDefinition<int>(CURRENT_PLAYER, 0);
        protected static readonly AttributeDefinition<int> TURN_COUNTER_DEF = new AttributeDefinition<int>(TURN_COUNTER, 1);
        protected static readonly AttributeDefinition<int> NEXT_PLAYER_ID_DEF = new AttributeDefinition<int>(NEXT_PLAYER_ID, 0);

        protected static readonly AttributeDefinition<int> NEXT_UNIT_ID_DEF = new AttributeDefinition<int>(NEXT_UNIT_ID, 0);

        public static readonly AttributeBuilder ATTRIBUTES_BUILDER = new AttributeBuilder(CURRENT_PLAYER_DEF, TURN_COUNTER_DEF, NEXT_PLAYER_ID_DEF, NEXT_UNIT_ID_DEF);

        private IAttributeContainer attributeContainer;

        //public virtual int CurrentPlayer { get; protected set; }
        //public virtual int TurnCounter { get; protected set; } = 1;

        protected Dictionary<int, Unit> _units { get; set; } = new Dictionary<int, Unit>();

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

        protected Dictionary<int, Player> _players { get; set; } = new Dictionary<int, Player>();

        public IReadOnlyDictionary<int, Player> Players
        {
            get { return _players; }
        }

        public IReadOnlyList<IAttribute> Attributes
        {
            get
            {
                return attributeContainer.Attributes;
            }
        }

        public BattleGameState()
        {
            
        }

        public void StartGame(int height, int width, Terrain[] terrain, Unit[] units, Player[] players, Dictionary<string, object> gameStateAttributes)
        {
            Contract.Requires<ArgumentNullException>(gameStateAttributes != null);

            Height = height;
            Width = width;

            _terrain = terrain;
            _units.Clear();
            foreach(var unit in units)
            {
                AddUnit(unit);
            }
            _players.Clear();
            foreach(var player in players)
            {
                AddPlayer(player);
            }
            attributeContainer = new AttributeContainer(ATTRIBUTES_BUILDER.BuildFullAttributeList(gameStateAttributes, true));
        }

        public override Unit GetUnit(int x, int y)
        {
            // TODO Improve performance?
            foreach(var unit in _units)
            {
                var loc = (unit.Value.GetAttribute(UnitType.LOCATION).GetValue() as Location);
                if (loc.X == x && loc.Y == y)
                {
                    return unit.Value;
                }
            }
            return null;
        }

        public Unit GetUnit(int unitId)
        {
            Unit unit;
            if (_units.TryGetValue(unitId, out unit))
            {
                return unit;
            }
            return null;
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
            throw new NotSupportedException();
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

        public override void AddUnit(Unit unit)
        {
            _units.Add(unit.UniqueId, unit);
        }

        public override void DeleteUnit(int x, int y)
        {
            DeleteUnit(GetUnit(x, y).UniqueId);
        }

        public void DeleteUnit(int id)
        {
            _units.Remove(id);
        }

        protected int nextPlayerId = Globals.UNIQUE_PLAYER_ID_RESERVED_NAMESPACE_END;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { attributeContainer.PropertyChanged += value; }
            remove { attributeContainer.PropertyChanged -= value; }
        }

        public event EventHandler<AttributeChangedEventArgs> AttributeChanged
        {
            add { attributeContainer.AttributeChanged += value; }
            remove { attributeContainer.AttributeChanged -= value; }
        }

        public int AddPlayer()
        {
            var id = nextPlayerId++;
            AddPlayer(id);
            return id;
        }

        public void AddPlayer(int id)
        {
            var p = new Player(id);
            AddPlayer(p);
        }

        protected void AddPlayer(Player player)
        {
            _players.Add(player.UniqueId, player);
        }

        public void RemovePlayer(int id)
        {
            _players.Remove(id);
        }

        public Player GetPlayer(int id)
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
            switch(changeInfo.ChangeCause)
            {
                case PlayerStateChange.Cause.Added:
                    AddPlayer(changeInfo.PlayerId);
                    break;
                case PlayerStateChange.Cause.Removed:
                    RemovePlayer(changeInfo.PlayerId);
                    return;
            }
            GetPlayer(changeInfo.PlayerId).SetAttributes(changeInfo.UpdatedAttributes);
        }

        public void UpdateUnit(UnitStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            switch (changeInfo.ChangeCause)
            {
                case UnitStateChange.Cause.Created:
                    AddUnit(new Unit(changeInfo.UpdatedAttributes));
                    break;
                case UnitStateChange.Cause.Destroyed:
                    DeleteUnit(changeInfo.UnitId);
                    return;
                case UnitStateChange.Cause.Changed:
                    var unit = GetUnit(changeInfo.UnitId);
                    if(null == unit)
                    {
                        throw new Exceptions.StateMismatchException(string.Format("Expected Unit with ID of {0} to exist, it did not", changeInfo.UnitId));
                    }
                    unit.SetAttributes(changeInfo.UpdatedAttributes);
                    return;
            }
        }

        public void UpdateTerrain(TerrainStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            var terrain = GetTerrain(changeInfo.ChangedTerrainLocation);
            if(null == terrain)
            {
                throw new Exceptions.StateMismatchException(
                    string.Format("Expected Terrain item to exist at {0},{1}, it did not", 
                    changeInfo.ChangedTerrainLocation.X, 
                    changeInfo.ChangedTerrainLocation.Y)
                );
            }

            terrain.SetAttributes(changeInfo.UpdatedAttributes);
        }

        public void UpdateGame(GameStateChange changeInfo)
        {
            SetAttributes(changeInfo.UpdatedAttributes);
        }

        public void Update(StateChange change)
        {
            if(change is GameStateChange)
            {
                UpdateGame(change as GameStateChange);
            }
            else if(change is PlayerStateChange)
            {
                UpdatePlayer(change as PlayerStateChange);
            }
            else if(change is UnitStateChange)
            {
                UpdateUnit(change as UnitStateChange);
            }
            else if(change is TerrainStateChange)
            {
                UpdateTerrain(change as TerrainStateChange);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unsupported StateChange type of {0}", change.GetType()));
            }
        }

        public IAttribute GetAttribute(string key)
        {
            return attributeContainer.GetAttribute(key);
        }

        public void SetAttribute(IAttribute value)
        {
            attributeContainer.SetAttribute(value);
        }

        public void SetAttributeReadOnly(string key)
        {
            attributeContainer.SetAttributeReadOnly(key);
        }

        public void SetAttributes(IReadOnlyList<IAttribute> values)
        {
            attributeContainer.SetAttributes(values);
        }
    }
}
