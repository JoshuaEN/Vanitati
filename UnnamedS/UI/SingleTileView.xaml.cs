using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for SingleTileView.xaml
    /// </summary>
    public partial class SingleTileView : UserControl
    {
        private Tile _tile;
        private MockBattleGameState mockState;

        public Commander Commander { get { return mockState?.Commander; } }
        public Unit Unit { get { return mockState?.Unit; } }
        public Terrain Terrain { get { return mockState?.Terrain; } }

        public SingleTileView()
        {
            InitializeComponent();
        }

        public void Set(Terrain terrain, Unit unit, Commander commander, double scale)
        {
            mockState = new MockBattleGameState(commander, unit, terrain);
            _tile = new Tile(new Tile.CanvasSet(renderCanvas), mockState, MockBattleGameState.Location, scale);
            Height = _tile.Height;
            Width = _tile.Width;
        }

        public void Update()
        {
            _tile?.UpdateLocation();
            _tile?.UpdateTerrain();
            _tile?.UpdateUnit();
        }

        public class MockBattleGameState : IReadOnlyBattleGameState
        {
            public static Location Location { get; } = new Location(0, 0);

            public Commander Commander { get; }
            public Unit Unit { get; }
            public Terrain Terrain { get; }

            public MockBattleGameState(Commander commander, Unit unit, Terrain terrain)
            {
                Commander = commander;
                Unit = unit;
                Terrain = terrain;
            }

            public IReadOnlyDictionary<int, Commander> Commanders
            {
                get
                {
                    if (Commander == null)
                        return new Dictionary<int, Commander>(0);
                    else
                        return new Dictionary<int, Commander>()
                        {
                            {Commander.CommanderID, Commander }
                        };
                }
            }

            public int CreditsPerCity
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Commander CurrentCommander
            {
                get
                {
                    return Commander;
                }
            }

            public int Height
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            IReadOnlyCollection<Terrain> IReadOnlyBattleGameState.Terrain
            {
                get
                {
                    return new List<Terrain>() { Terrain };
                }
            }

            public int TurnID
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IReadOnlyDictionary<int, Unit> Units
            {
                get
                {
                    if (Unit == null)
                        return new Dictionary<int, Unit>(0);
                    else
                        return new Dictionary<int, Unit>()
                        {
                            {Unit.UnitID, Unit }
                        };
                }
            }

            public int Width
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int VictoryPointsPerPoint
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointLimitEnabled
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int VictoryPointLimit
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointGapEnabled
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int VictoryPointGap
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IReadOnlyList<GameAction> Actions
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointVictoryAchieved
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public BattleGameState Fork()
            {
                throw new NotImplementedException();
            }

            public Commander GetCommander(int commanderID)
            {
                if (commanderID == Commander?.CommanderID)
                    return Commander;
                else
                    return null;
            }

            public BattleGameState.Fields GetFields()
            {
                throw new NotImplementedException();
            }

            public int GetNextUnitID()
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, object> GetProperties()
            {
                throw new NotImplementedException();
            }

            public Terrain GetTerrain(Location loc)
            {
                if (loc == Location)
                    return Terrain;
                else
                    return null;
            }

            public Terrain GetTerrain(int x, int y)
            {
                return GetTerrain(new Location(x, y));
            }

            public Game.Tile GetTile(Location loc)
            {
                return new Game.Tile(GetTerrain(loc), GetUnit(loc));
            }

            public Game.Tile GetTile(int x, int y)
            {
                return GetTile(new Location(x, y));
            }

            public Unit GetUnit(int unitId)
            {
                if (Unit?.UnitID == unitId)
                    return Unit;
                else
                    return null;
            }

            public Unit GetUnit(Location loc)
            {
                if (Location == loc)
                    return Unit;
                else
                    return null;
            }

            public Unit GetUnit(int x, int y)
            {
                return GetUnit(new Location(x, y));
            }

            public IDictionary<string, object> GetWriteableProperties()
            {
                throw new NotImplementedException();
            }

            public bool IsCommanderFriendly(int ourCommanderID, int otherCommanderID)
            {
                return ourCommanderID == otherCommanderID;
            }

            public bool LocationsAdjacent(Location a, Location b)
            {
                throw new NotImplementedException();
            }

            public List<Location> LocationsAroundPoint(Location point, int range)
            {
                throw new NotImplementedException();
            }

            public List<Location> LocationsAroundPoint(Location point, int minimum, int maximum)
            {
                throw new NotImplementedException();
            }

            public Commander SafeGetCommander(int commanderID)
            {
                throw new NotImplementedException();
            }

            public void SetProperties(IDictionary<string, object> values)
            {
                throw new NotImplementedException();
            }
        }
    }
}
