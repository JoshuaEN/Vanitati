using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Game
{
    public class BattleGameState : GameState, IPropertyContainer, IReadOnlyBattleGameState
    {
        [JsonProperty]
        public int CurrentCommanderIndex { get; set; } = -1;

        [JsonProperty]
        public int TurnCounter { get; set; } = 1;

        [JsonProperty]
        public int NextUnitID { get; set; } = 0;

        [JsonIgnore]
        public Commander CurrentCommander
        {
            get
            {
                if (CurrentCommanderIndex == -1)
                    return null;

                return _commanderOrder[CurrentCommanderIndex];
            }
        }

        [JsonProperty]
        protected Dictionary<int, Unit> _units { get; set; } = new Dictionary<int, Unit>();

        /// <summary>
        /// Listing of all units on the map, the int being the unique ID of the unit.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyDictionary<int, Unit> Units
        {
            get { return _units; }
        }

        [JsonProperty]
        protected Terrain[] _terrain { get; set; } = new Terrain[0];

        [JsonIgnore]
        public IReadOnlyCollection<Terrain> Terrain
        {
            get
            {
                if (_terrain == null)
                    return null;

                return Array.AsReadOnly(_terrain);
            }
        }

        [JsonProperty]
        protected Commander[] _commanderOrder { get; set; } = new Commander[0];

        [JsonProperty]
        protected Dictionary<int, Commander> _commanders { get; set; } = new Dictionary<int, Commander>();

        [JsonIgnore]
        public IReadOnlyDictionary<int, Commander> Commanders
        {
            get { return _commanders; }
        }

        [JsonConstructor]
        public BattleGameState()
        {

        }

        public void Sync(Fields fields)
        {
            Contract.Requires<ArgumentNullException>(fields != null);

            Height = fields.Height;
            Width = fields.Width;

            _terrain = fields.Terrain;
            _units.Clear();
            foreach (var unit in fields.Units)
            {
                AddUnit(unit);
            }
            _commanders.Clear();
            foreach (var commander in fields.Commanders)
            {
                _commanders.Add(commander.CommanderID, commander);
            }
            _commanderOrder = fields.Commanders;
            SetProperties(fields.Values);
        }

        public void StartGame(Fields fields)
        {
            Contract.Requires<ArgumentNullException>(fields != null);

            Sync(fields);
            AdvanceToNextCommander();
        }

        public override Unit GetUnit(int x, int y)
        {
            // TODO Improve performance?
            foreach(var unit in _units)
            {
                var loc = unit.Value.Location;
                if (loc.X == x && loc.Y == y)
                {
                    return unit.Value;
                }
            }
            return null;
        }

        Unit IReadOnlyBattleGameState.GetUnit(int x, int y)
        {
            return GetUnit(x, y);
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

        Terrain IReadOnlyBattleGameState.GetTerrain(int x, int y)
        {
            return GetTerrain(x, y);
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
            Contract.Requires<ArgumentException>(0 <= x, "X Cord should be >= 0");
            Contract.Requires<ArgumentException>(0 <= y, "Y Cord should be >= 0");
            Contract.Ensures(Contract.Result<int>() >= 0, "Resulting index should be >= 0");
            Contract.Ensures(Contract.Result<int>() <= _terrain.Length, "Resulting index should be <= game board size");

            var i = y + Height * x;

            if(i >= _terrain.Length)
            {
                throw new IndexOutOfRangeException("X Y Coordinate outside of valid range.");
            }

            return i;
        }

        public override void AddUnit(Unit unit)
        {
            _units.Add(unit.UnitID, unit);
        }

        public override void DeleteUnit(int x, int y)
        {
            DeleteUnit(GetUnit(x, y).UnitID);
        }

        public void DeleteUnit(int id)
        {
            _units.Remove(id);
        }

        public Commander GetCommander(int commanderID)
        {
            Commander p;
            if(_commanders.TryGetValue(commanderID, out p) == false)
            {
                throw new Exceptions.UnknownPlayerException();
            }

            return p;
        }

        public virtual void EndTurn(int commanderID)
        {
            if(CurrentCommander.CommanderID != commanderID)
            {
                throw new Exceptions.NotYourTurnException();
            }

            AdvanceToNextCommander();
        }

        private void AdvanceToNextCommander()
        {
            CurrentCommanderIndex++;
            if (CurrentCommanderIndex >= _commanderOrder.Length)
                CurrentCommanderIndex = 0;
            else if (CurrentCommanderIndex < 0)
                CurrentCommanderIndex = 0;
        }

        public Fields GetFields()
        {
            return new Fields(Height, Width, _terrain.ToArray(), _units.Values.ToArray(), _commanders.Values.ToArray(), GetWriteableProperties(), CurrentCommanderIndex);
        }

        public void UpdateCommander(CommanderStateChange changeInfo)
        {
            switch(changeInfo.ChangeCause)
            {
                case CommanderStateChange.Cause.Added:
                    throw new NotSupportedException();
                case CommanderStateChange.Cause.Removed:
                    throw new NotSupportedException();
                case CommanderStateChange.Cause.Changed:
                    var commander = GetCommander(changeInfo.CommanderID);

                    if (null == commander)
                    {
                        throw new Exceptions.StateMismatchException(string.Format("Expected Unit with ID of {0} to exist, it did not", changeInfo.CommanderID));
                    }
                    commander.SetProperties(changeInfo.UpdatedProperties);
                    return;
                default:
                    throw new InvalidOperationException("Unknown Change Cause: " + changeInfo.ChangeCause);
            }
        }

        public void UpdateUnit(UnitStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            switch (changeInfo.ChangeCause)
            {
                case UnitStateChange.Cause.Created:
                    AddUnit(new Unit(changeInfo.UpdatedProperties));
                    break;
                case UnitStateChange.Cause.Destroyed:
                    DeleteUnit(changeInfo.UnitID);
                    return;
                case UnitStateChange.Cause.Changed:
                    var unit = GetUnit(changeInfo.UnitID);
                    if(null == unit)
                    {
                        throw new Exceptions.StateMismatchException(string.Format("Expected Unit with ID of {0} to exist, it did not", changeInfo.UnitID));
                    }
                    unit.SetProperties(changeInfo.UpdatedProperties);
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

            terrain.SetProperties(changeInfo.UpdatedProperties);
        }

        public void UpdateGame(GameStateChange changeInfo)
        {
            SetProperties(changeInfo.UpdatedProperties);
        }

        public void Update(StateChange change)
        {
            if(change is GameStateChange)
            {
                UpdateGame(change as GameStateChange);
            }
            else if(change is CommanderStateChange)
            {
                UpdateCommander(change as CommanderStateChange);
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

        

        public bool LocationsAdjacent(Location a, Location b)
        {
            return LocationsAroundPoint(a, 1).Contains(b);
        }

        public List<Location> LocationsAroundPoint(Location point, int minimum, int maximum)
        {
            var list = new List<Location>();

            for(var i = minimum; i <= maximum; i++)
            {
                list.AddRange(LocationsAroundPoint(point, i));
            }

            return list;            
        }

        public List<Location> LocationsAroundPoint(Location point, int range)
        {
            if (range == 0)
                return new List<Location>() { point };

            var list = new List<Location>();
            var x = point.X;
            var y = point.Y;

            var xEdge = x - range;

            // Intentional integer division
            var yEdge = y - (range / 2);

            if (range % 2 != 0 && x % 2 == 0)
                yEdge -= 1;

            var xReverse = xEdge + range * 2;

            var sideLength = (range + 1);

            for(var c = 0; c < sideLength; c++)
            {
                // "wall" on left side
                list.Add(new Location(xEdge, yEdge + c));
                // "wall" on right side
                list.Add(new Location(xReverse, yEdge + c));
            }

            for (var d = 1; d < range; d++)
            {
                // Intentional integer division
                var tEdge = yEdge - (d / 2);

                if (range % 2 == 0)
                {
                    if (x % 2 == 0 && (xEdge + d) % 2 != 0)
                        tEdge -= 1;
                }
                else
                {
                    if (x % 2 != 0 && (xEdge + d) % 2 != 0)
                        tEdge -= 1;
                }

                // top-left
                list.Add(new Location(xEdge + d, tEdge));
                // top-right
                list.Add(new Location(xReverse - d, tEdge));
                // bottom-left
                list.Add(new Location(xEdge + d, tEdge + range + d));
                // bottom-right
                list.Add(new Location(xReverse - d, tEdge + range + d));
            }

            // directly above
            list.Add(new Location(x, y + range));
            // directly below
            list.Add(new Location(x, y - range));

            return list;
        }

        public IDictionary<string, object> GetProperties()
        {
            return PropertyContainer.BATTLE_GAME_STATE.GetProperties(this);
        }

        public IDictionary<string, object> GetWriteableProperties()
        {
            return PropertyContainer.BATTLE_GAME_STATE.GetWriteableProperties(this);
        }

        public void SetProperties(IDictionary<string, object> values)
        {
            PropertyContainer.BATTLE_GAME_STATE.SetProperties(this, values);
        }

        public BattleGameState Fork()
        {
            // TODO Perhaps not the best way.
            return Serializers.Serializer.Deserialize<BattleGameState>(Serializers.Serializer.Serialize(this));
        }


        public class Fields
        {
            public int Height { get; }
            public int Width { get; }
            public Terrain[] Terrain { get; }
            public Unit[] Units { get; }
            public Commander[] Commanders { get; }

            [Newtonsoft.Json.JsonConverter(typeof(Serializers.JsonConverters.DynamicProperitesConverter))]
            public IDictionary<string, object> Values { get; }

            public int CurrentCommanderIndex { get; }

            public Fields(int height, int width, Terrain[] terrain, Unit[] units, Commander[] commanders, IDictionary<string, object> values, int currentCommanderIndex)
            {
                Height = height;
                Width = width;
                Terrain = terrain;
                Units = units;
                Commanders = commanders;
                Values = values;
                CurrentCommanderIndex = currentCommanderIndex;

            }
        }
        //public class GridFlower
        //{
        //    public Location Top { get; }
        //    public Location Bottom { get; }
        //    public Location TopLeft { get; }
        //    public Location TopRight { get; }
        //    public Location BottomLeft { get; }
        //    public Location BottomRight { get; }
        //    public Location Center { get; }

        //    public GridFlower(Location center)
        //    {
        //        Contract.Requires<ArgumentNullException>(null != center);

        //        var x = center.X;
        //        var y = center.Y;

        //        Top = new Location(x, y - 1);
        //        Bottom = new Location(x, y + 1);

        //        if (x % 2 == 0)
        //        {
        //            TopLeft = new Location(x - 1, y - 1);
        //            TopRight = new Location(x, y - 1);
        //            BottomLeft = new Location(x - 1, y + 1);
        //            BottomRight = new Location(x, y + 1);
        //        }
        //        else
        //        {
        //            TopLeft = new Location(x, y - 1);
        //            TopRight = new Location(x + 1, y - 1);
        //            BottomLeft = new Location(x, y + 1);
        //            BottomRight = new Location(x + 1, y + 1);
        //        }
        //        Center = center;
        //    }

        //    public Location GetAtPedal(Pedal pedal)
        //    {
        //        switch(pedal)
        //        {
        //            case Pedal.Top:
        //                return Top;
        //            case Pedal.Bottom:
        //                return Bottom;
        //            case Pedal.TopLeft:
        //                return TopLeft;
        //            case Pedal.TopRight:
        //                return TopRight;
        //            case Pedal.BottomLeft:
        //                return BottomLeft;
        //            case Pedal.BottomRight:
        //                return BottomRight;
        //            case Pedal.Center:
        //                return Center;
        //            case Pedal.None:
        //                return null;
        //            default:
        //                throw new ArgumentException("Unknown Pedal direction of " + pedal);
        //        }
        //    }

        //    public Pedal GetAtLocation(Location loc)
        //    {
        //        if (Math.Abs(loc.X - Center.X) > 1)
        //            return Pedal.None;
        //        if (Math.Abs(loc.Y - Center.Y) > 2)
        //            return Pedal.None;

        //        if (loc == Top)
        //            return Pedal.Top;
        //        if (loc == Bottom)
        //            return Pedal.Bottom;
        //        if (loc == TopLeft)
        //            return Pedal.TopLeft;
        //        if (loc == TopRight)
        //            return Pedal.TopRight;
        //        if (loc == BottomLeft)
        //            return Pedal.BottomLeft;
        //        if (loc == BottomRight)
        //            return Pedal.BottomRight;
        //        if (loc == Center)
        //            return Pedal.Center;

        //        return Pedal.None;
        //    }

        //    public bool IsInPedal(Location loc)
        //    {
        //        return GetAtLocation(loc) != Pedal.None;
        //    }

        //    public enum Pedal { Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight, Center, None }
        //}
    }
}
