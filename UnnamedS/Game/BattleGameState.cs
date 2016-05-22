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
        #region Properties

        [JsonIgnore]
        public IReadOnlyList<ActionTypes.GameAction> Actions { get; } = new List<ActionTypes.GameAction>()
        {
            ActionTypes.ForGame.DetectVictoryPointWinConditions.Instance
        };

        [JsonProperty]
        public int CurrentCommanderIndex { get; set; } = -1;

        [JsonProperty]
        public int TurnID { get; set; } = 1;

        [JsonProperty]
        public int NextUnitID { get; set; } = 0;

        [JsonProperty]
        public int CreditsPerCity { get; private set; } = 1000;

        [JsonProperty]
        public int VictoryPointsPerPoint { get; private set; } = 1;

        [JsonProperty]
        public bool VictoryPointLimitEnabled { get; private set; } = false;

        [JsonProperty]
        public int VictoryPointLimit { get; private set; } = 50;

        [JsonProperty]
        public bool VictoryPointGapEnabled { get; private set; } = true;

        [JsonProperty]
        public int VictoryPointGap { get; private set; } = 20;

        [JsonProperty]
        public bool VictoryPointVictoryAchieved { get; private set; } = false;

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

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected Dictionary<int, Unit> _units { get; set; } = new Dictionary<int, Unit>();

        [JsonIgnore]
        protected Dictionary<Location, Unit> _unitsByLocation { get; set; } = new Dictionary<Location, Unit>();

        /// <summary>
        /// Listing of all units on the map, the int being the unique ID of the unit.
        /// </summary>
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IReadOnlyDictionary<int, Unit> Units
        {
            get { return _units; }
        }

        [JsonIgnore]
        protected Terrain[] _terrain { get; set; } = new Terrain[0];

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected Dictionary<int, Commander> _commanders { get; set; } = new Dictionary<int, Commander>();

        [JsonIgnore]
        public IReadOnlyDictionary<int, Commander> Commanders
        {
            get { return _commanders; }
        }

        #endregion

        [JsonConstructor]
        public BattleGameState()
        {

        }

        public void Sync(Fields fields)
        {
            Contract.Requires<ArgumentNullException>(fields != null);

            SetProperties(fields.Values);

            Height = fields.Height;
            Width = fields.Width;

            _terrain = fields.Terrain;
            _units.Clear();
            _unitsByLocation.Clear();
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

            CurrentCommanderIndex = fields.CurrentCommanderIndex;

        }

        public void StartGame(Fields fields, StartMode startMode)
        {
            Contract.Requires<ArgumentNullException>(fields != null);

            Sync(fields);

            if (startMode == StartMode.NewGame)
            {
                CurrentCommanderIndex = -1;
                AdvanceToNextCommander();
            }
        }

        public override Unit GetUnit(int x, int y)
        {
            return GetUnit(new Location(x, y));
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

        public override Unit GetUnit(Location loc)
        {
            Unit unit;
            if (_unitsByLocation.TryGetValue(loc, out unit) == false)
                return null;

            return unit;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
            int idx;

            if (TryGetIndex(x, y, out idx) == false)
                return null;
            else
                return _terrain[idx];
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Terrain IReadOnlyBattleGameState.GetTerrain(int x, int y)
        {
            return GetTerrain(x, y);
        }

        private bool TryGetIndex(int x, int y, out int index)
        {
            index = y + Height * x;

            if (x < 0 || y < 0 || index >= _terrain.Length)
            {
                index = -1;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override Tile GetTile(int x, int y)
        {
            int idx;

            if (TryGetIndex(x, y, out idx) == false)
                return null;

            return new Tile(GetTerrain(x, y), GetUnit(x, y));
        }

        public static int GetIndex(int x, int y, int height)
        {
            return y + height * x;
        }

        public int GetNextUnitID()
        {
            return NextUnitID++;
        }

        public override void AddUnit(Unit unit)
        {
            if (NextUnitID <= unit.UnitID)
                NextUnitID = unit.UnitID + 1;

            _units.Add(unit.UnitID, unit);
            _unitsByLocation[unit.Location] = unit;
        }

        public override void DeleteUnit(int x, int y)
        {
            var unit = GetUnit(x, y);

            if(null != unit)
                DeleteUnit(unit.UnitID);
        }

        public void DeleteUnit(int unitID)
        {
            Unit unit;

            if (_units.TryGetValue(unitID, out unit))
            {
                _units.Remove(unitID);
                _unitsByLocation.Remove(unit.Location);
            }
        }

        public Commander GetCommander(int commanderID)
        {
            var commander = SafeGetCommander(commanderID);

            if(commander == null)
                throw new Exceptions.UnknownCommanderException();

            return commander;
        }

        public Commander SafeGetCommander(int commanderID)
        {
            Commander p;
            if (_commanders.TryGetValue(commanderID, out p) == false)
            {
                return null;
            }

            return p;
        }

        public bool IsCommanderFriendly(int ourCommanderID, int otherCommanderID)
        {
            return ourCommanderID == otherCommanderID;
        }

        public virtual void EndTurn(int commanderID)
        {
            if(CurrentCommander == null)
            {
                throw new Exceptions.NotYourTurnException();
            }
            else if(CurrentCommander.CommanderID != commanderID)
            {
                throw new Exceptions.NotYourTurnException();
            }

            AdvanceToNextCommander();
            TurnID += 1;
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
            return new Fields(
                Height, 
                Width, 
                _terrain.ToArray(), 
                _units.Values.ToArray(), 
                _commanders.Values.ToArray(), 
                GetWriteableProperties(), 
                CurrentCommanderIndex
            );
        }

        public void UpdateCommander(CommanderStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            switch(changeInfo.ChangeCause)
            {
                case CommanderStateChange.Cause.Changed:
                    var commander = GetCommander(changeInfo.CommanderID);

                    if (null == commander)
                    {
                        throw new Exceptions.StateMismatchException(string.Format("Expected Commander with ID of {0} to exist, it did not", changeInfo.CommanderID));
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
                case UnitStateChange.Cause.Added:
                    AddUnit(new Unit(changeInfo.UpdatedProperties));
                    break;
                case UnitStateChange.Cause.Destroyed:
                case UnitStateChange.Cause.Removed:
                    DeleteUnit(changeInfo.UnitID);
                    return;
                case UnitStateChange.Cause.Changed:
                    var unit = GetUnit(changeInfo.UnitID);
                    if(null == unit)
                    {
                        throw new Exceptions.StateMismatchException(string.Format("Expected Unit with ID of {0} to exist, it did not", changeInfo.UnitID));
                    }
                    unit.SetProperties(changeInfo.UpdatedProperties);

                    if(changeInfo.LocationChanged)
                    {
                        Unit unitAtPrev;
                        if (_unitsByLocation.TryGetValue(changeInfo.PreviousLocation, out unitAtPrev) && unitAtPrev.UnitID == unit.UnitID)
                            _unitsByLocation.Remove(changeInfo.PreviousLocation);

                        _unitsByLocation[changeInfo.Location] = unit;
                    }

                    return;
            }
        }

        public void UpdateTerrain(TerrainStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

            var terrain = GetTerrain(changeInfo.Location);
            if(null == terrain)
            {
                throw new Exceptions.StateMismatchException(
                    string.Format("Expected Terrain item to exist at {0},{1}, it did not", 
                    changeInfo.Location.X, 
                    changeInfo.Location.Y)
                );
            }

            terrain.SetProperties(changeInfo.UpdatedProperties);
        }

        public void UpdateGame(GameStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

            SetProperties(changeInfo.UpdatedProperties);
        }

        public void Update(StateChange changeInfo)
        {
            if(changeInfo is UnitStateChange)
            {
                UpdateUnit(changeInfo as UnitStateChange);
            }
            else if(changeInfo is TerrainStateChange)
            {
                UpdateTerrain(changeInfo as TerrainStateChange);
            }
            else if(changeInfo is CommanderStateChange)
            {
                UpdateCommander(changeInfo as CommanderStateChange);
            }
            else if(changeInfo is GameStateChange)
            {
                UpdateGame(changeInfo as GameStateChange);
            }
            else
            {
                throw new ArgumentException($"Unknown state change type of {changeInfo.GetType()}");
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

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IDictionary<string, object> GetProperties()
        {
            return PropertyContainer.BATTLE_GAME_STATE.GetProperties(this);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IDictionary<string, object> GetWriteableProperties()
        {
            return PropertyContainer.BATTLE_GAME_STATE.GetWriteableProperties(this);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public void SetProperties(IDictionary<string, object> values)
        {
            PropertyContainer.BATTLE_GAME_STATE.SetProperties(this, values);
        }

        public BattleGameState Fork()
        {
            // TODO Perhaps not the best way.
            var newState = new BattleGameState();
            newState.Sync(Serializers.Serializer.Deserialize<BattleGameState.Fields>(Serializers.Serializer.Serialize(GetFields())));
            return newState;
        }


        public class Fields
        {
            public int Height { get; }
            public int Width { get; }
            public Terrain[] Terrain { get; }
            public Unit[] Units { get; }
            public Commander[] Commanders { get; }

            [JsonConverter(typeof(Serializers.JsonConverters.DynamicProperitesConverter))]
            public IDictionary<string, object> Values { get; }

            public int CurrentCommanderIndex { get; }

            public Fields(int height, int width, Terrain[] terrain, Unit[] units, Commander[] commanders, IDictionary<string, object> values, int currentCommanderIndex)
            {
                Contract.Requires<ArgumentNullException>(null != terrain);
                Contract.Requires<ArgumentNullException>(null != units);
                Contract.Requires<ArgumentNullException>(null != commanders);
                Contract.Requires<ArgumentNullException>(null != values);

                Height = height;
                Width = width;
                Terrain = terrain;
                Units = units;
                Commanders = commanders;
                Values = values;
                CurrentCommanderIndex = currentCommanderIndex;
            }

            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(null != Terrain);
                Contract.Invariant(null != Units);
                Contract.Invariant(null != Commanders);
                Contract.Invariant(null != Values);
            }
        }

        public enum StartMode { NewGame, LoadedSaveGame }
    }
}
