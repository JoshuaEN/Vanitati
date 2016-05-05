using Xunit;
using UnnamedStrategyGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Tests
{
    public class BattleGameStateTests
    {
        public BattleGameStateTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void SyncTest()
        {
            var state = new BattleGameState();

            var fields = GetFields();

            state.Sync(fields);


            CrossCheckFieldsWithState(fields, state);
        }

        [Fact()]
        public void StartGameTest()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            
            state.StartGame(fields);

            fields = new BattleGameState.Fields(fields.Height, fields.Width, fields.Terrain, fields.Units, fields.Commanders, fields.Values, 1);

            CrossCheckFieldsWithState(fields, state);
        }

        [Fact()]
        public void GetExistingUnitTestXY()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var unit = state.GetUnit(0, 0);
            Assert.Equal(fields.Units[0], unit);
        }

        [Fact()]
        public void GetNotExistingUnitTestXY()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var unit = state.GetUnit(1, 0);
            Assert.Equal(null, unit);
        }

        [Fact()]
        public void GetExistingUnitTestUnitID()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var unit = state.GetUnit(fields.Units[0].UnitID);
            Assert.Equal(fields.Units[0], unit);
        }

        [Fact()]
        public void GetNotExistingUnitTestUnitID()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var unit = state.GetUnit(99);
            Assert.Equal(null, unit);
        }

        [Fact()]
        public void GetExistingTerrainTestXY()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var terrain = state.GetTerrain(0, 0);
            Assert.Equal(fields.Terrain[0], terrain);
        }

        [Fact()]
        public void GetNotExistingTerrainTestXY()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var terrain = state.GetTerrain(fields.Height+1, 0);
            Assert.Equal(null, terrain);
        }

        [Fact()]
        public void GetTileTest_ExistingWithUnit()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var tile = state.GetTile(0, 0);
            Assert.NotEqual(null, tile);
            Assert.Equal(fields.Terrain[0], tile.Terrain);
            Assert.Equal(fields.Units[0], tile.Unit);
            Assert.Equal(tile.Location, tile.Unit.Location);
            Assert.Equal(tile.Location, tile.Terrain.Location);
        }

        [Fact()]
        public void GetTileTest_ExistingWithOutUnit()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var tile = state.GetTile(1, 0);
            Assert.NotEqual(null, tile);
            Assert.Equal(fields.Terrain[BattleGameState.GetIndex(1, 0, fields.Height)], tile.Terrain);
            Assert.Equal(null, tile.Unit);
            Assert.Equal(tile.Location, tile.Terrain.Location);
        }

        [Fact()]
        public void GetTileTest_NotExisting()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var tile = state.GetTile(fields.Height+1, 0);
            Assert.Equal(null, tile);
        }

        [Fact()]
        public void GetNextUnitIDTest()
        {
            var state = GetState();

            int next = state.GetNextUnitID();

            Assert.Equal(state.Units.Max(kp => kp.Key) + 1, next);
        }

        [Fact()]
        public void AddUnitTest_Valid()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;
            var unit = new Unit(state.GetNextUnitID(), UnitTypes.Infantry.Instance, new Location(1,0), state.Commanders[0].CommanderID);
            state.AddUnit(unit);

            Assert.Equal(priorUnitCount + 1, state.Units.Count);
            Assert.Contains(unit, state.Units.Values);
            Assert.Equal(unit, state.GetUnit(unit.UnitID));
            Assert.Equal(unit, state.GetUnit(unit.Location));
        }

        [Fact()]
        public void AddUnitTest_TakenLocation()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;
            var unit = new Unit(state.GetNextUnitID(), UnitTypes.Infantry.Instance, new Location(0, 0), state.Commanders[0].CommanderID);

            Assert.Throws<ArgumentException>(() =>
            {
                state.AddUnit(unit);
            });
            Assert.Equal(priorUnitCount, state.Units.Count);
        }

        [Fact()]
        public void AddUnitTest_TakenUnitID()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;
            var unit = new Unit(state.Units.First().Key, UnitTypes.Infantry.Instance, new Location(0, 0), state.Commanders[0].CommanderID);

            Assert.Throws<ArgumentException>(() =>
            {
                state.AddUnit(unit);
            });
            Assert.Equal(priorUnitCount, state.Units.Count);
        }

        [Fact()]
        public void DeleteUnitTest_XY_Valid()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;
            var unit = state.Units.First().Value;

            state.DeleteUnit(unit.Location.X, unit.Location.Y);
            Assert.Equal(priorUnitCount - 1, state.Units.Count);
            Assert.DoesNotContain(unit, state.Units.Values);
        }

        [Fact()]
        public void DeleteUnitTest_XY_MissingUnit()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;

            state.DeleteUnit(1, 0);
            Assert.Equal(priorUnitCount, state.Units.Count);
        }

        [Fact()]
        public void DeleteUnitTest_UnitID_Valid()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;
            var unit = state.Units.First().Value;

            state.DeleteUnit(unit.UnitID);
            Assert.Equal(priorUnitCount - 1, state.Units.Count);
            Assert.DoesNotContain(unit, state.Units.Values);
        }

        [Fact()]
        public void DeleteUnitTest_UnitID_MissingUnit()
        {
            var state = GetState();
            var priorUnitCount = state.Units.Count;

            state.DeleteUnit(state.GetNextUnitID());
            Assert.Equal(priorUnitCount, state.Units.Count);
        }

        [Fact()]
        public void GetCommanderTest_Valid()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var commander = state.GetCommander(fields.Commanders[0].CommanderID);

            Assert.Equal(fields.Commanders[0], commander);
        }

        [Fact()]
        public void GetCommanderTest_MissingCommander()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            Assert.Throws<Exceptions.UnknownCommanderException>(() =>
            {
                var commander = state.GetCommander(99);
            });
        }

        [Fact]
        public void SafeGetCommanderTest_Valid()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            var commander = state.SafeGetCommander(fields.Commanders[0].CommanderID);

            Assert.Equal(fields.Commanders[0], commander);
        }

        [Fact]
        public void SafeGetCommanderTest_MissingCommander()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);

            Assert.Equal(null, state.SafeGetCommander(99));
        }

        [Fact()]
        public void EndTurnTest_Valid()
        {
            var state = GetState();
            var commanderBefore = state.CurrentCommander;

            state.EndTurn(commanderBefore.CommanderID);

            Assert.NotEqual(commanderBefore, state.CurrentCommander);
        }

        [Fact()]
        public void EndTurnTest_NotCommandersTurn()
        {
            var state = GetState();
            var commanderBefore = state.CurrentCommander;

            Assert.Throws<Exceptions.NotYourTurnException>(() =>
            {
                state.EndTurn(state.Commanders.Keys.Where(cmdID => cmdID != commanderBefore.CommanderID).First());
            });

            Assert.Equal(commanderBefore, state.CurrentCommander);
        }

        [Fact]
        public void EndTurnTest_WrapAround()
        {
            var state = GetState();
            var commanderBefore = state.CurrentCommander;

            state.EndTurn(state.CurrentCommander.CommanderID);

            Assert.Equal(0, state.CurrentCommander.CommanderID);
        }

        [Fact]
        public void EndTurnTest_LowerBoundAdjust()
        {
            var fields = GetFields();
            fields = new BattleGameState.Fields(fields.Height, fields.Width, fields.Terrain, fields.Units, fields.Commanders, fields.Values, -99);

            var state = new BattleGameState();
            state.StartGame(fields);
            
            Assert.Equal(0, state.CurrentCommander.CommanderID);
        }

        [Fact]
        public void EndTurnTest_GameNotStarted()
        {
            var state = new BattleGameState();
            var fields = GetFields();
            state.Sync(fields);
            state.CurrentCommanderIndex = -1;

            Assert.Throws<Exceptions.NotYourTurnException>(() =>
            {
                state.EndTurn(0);
            });
        }

        [Fact()]
        public void GetFieldsTest()
        {
            var fields = GetFields();
            var state = new BattleGameState();
            state.Sync(fields);

            var otherFields = state.GetFields();

            CrossCheckFields(fields, otherFields);
        }

        [Fact]
        public void TryGetIndexTest()
        {
            var state = GetState();

            Assert.NotEqual(null, state.GetTerrain(0, 0));
            Assert.Equal(null, state.GetTerrain(-1, 0));
            Assert.Equal(null, state.GetTerrain(0, -1));
            Assert.Equal(null, state.GetTerrain(5, 5));
        }

        //[Fact()]
        //public void UpdateCommanderTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void UpdateUnitTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void UpdateTerrainTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void UpdateGameTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        [Fact()]
        public void LocationsAdjacentTest_Valid()
        {
            var point = new Location(7, 3);
            var around = new List<Location>()
            {
                new Location(7,2),
                new Location(6,3),
                new Location(6,4),
                new Location(7,4),
                new Location(8,4),
                new Location(8,3)
            };

            var state = GetState();

            foreach(var loc in around)
            {
                Assert.True(state.LocationsAdjacent(point, loc), "{loc} is next to {point}");
            }

            point = new Location(6, 4);
            around = new List<Location>()
            {
                new Location(6,3),
                new Location(5,3),
                new Location(5,4),
                new Location(6,5),
                new Location(7,4),
                new Location(7,3)
            };

            foreach (var loc in around)
            {
                Assert.True(state.LocationsAdjacent(point, loc), "{loc} is next to {point}");
            }
        }

        [Fact()]
        public void LocationsAdjacentTest_Invalid()
        {
            var point = new Location(7, 3);
            var around = new List<Location>()
            {
                new Location(7,2),
                new Location(6,3),
                new Location(6,4),
                new Location(7,4),
                new Location(8,4),
                new Location(8,3)
            };

            var state = GetState();

            foreach (var loc in LocationsAroundTheseButNotThem(around))
            {
                Assert.False(state.LocationsAdjacent(point, loc), "{loc} is not next to {point}");
            }

            point = new Location(6, 4);
            around = new List<Location>()
            {
                new Location(6,3),
                new Location(5,3),
                new Location(5,4),
                new Location(6,5),
                new Location(7,4),
                new Location(7,3)
            };

            foreach (var loc in LocationsAroundTheseButNotThem(around))
            {
                Assert.False(state.LocationsAdjacent(point, loc), "{loc} not is next to {point}");
            }
        }

        private HashSet<Location> LocationsAroundTheseButNotThem(List<Location> locations)
        {
            var list = new HashSet<Location>();
            foreach (var loc in locations)
            {
                for (var i = -3; i <= 3; i++)
                {
                    var newLocX = new Location(loc.X + i, loc.Y);
                    var newLocY = new Location(loc.X, loc.Y + i);

                    if (locations.Contains(newLocX) == false)
                        list.Add(newLocX);
                    if (locations.Contains(newLocY) == false)
                        list.Add(newLocY);
                }
            }
            return list;
        }

        [Fact()]
        public void LocationsAroundPointTest()
        {
            var state = GetState();

            Func<Location, int> compare = loc => BattleGameState.GetIndex(loc.X, loc.Y, 200);

            var point = new Location(7, 3);
            var at0 = new List<Location>()
            {
                new Location(7, 3)
            };
            var at1 = new List<Location>()
            {
                new Location(7,2),
                new Location(6,3),
                new Location(6,4),
                new Location(7,4),
                new Location(8,4),
                new Location(8,3)
            }.OrderBy(compare);
            var at2 = new List<Location>()
            {
                new Location(7,1),
                new Location(6,2),
                new Location(5,2),
                new Location(5,3),
                new Location(5,4),
                new Location(6,5),
                new Location(7,5),
                new Location(8,5),
                new Location(9,4),
                new Location(9,3),
                new Location(9,2),
                new Location(8,2)
            }.OrderBy(compare);
            var at3 = new List<Location>()
            {
                new Location(7,0),
                new Location(6,1),
                new Location(5,1),
                new Location(4,2),
                new Location(4,3),
                new Location(4,4),
                new Location(4,5),
                new Location(5,5),
                new Location(6,6),
                new Location(7,6),
                new Location(8,6),
                new Location(9,5),
                new Location(10,5),
                new Location(10,4),
                new Location(10,3),
                new Location(10,2),
                new Location(9,1),
                new Location(8,1)
            }.OrderBy(compare);

            Assert.Equal(at0, state.LocationsAroundPoint(point, 0).OrderBy(compare));
            Assert.Equal(at1, state.LocationsAroundPoint(point, 1).OrderBy(compare));
            Assert.Equal(at2, state.LocationsAroundPoint(point, 2).OrderBy(compare));
            Assert.Equal(at3, state.LocationsAroundPoint(point, 3).OrderBy(compare));

            point = new Location(6, 4);
            at0 = new List<Location>()
            {
                new Location(6,4)
            };
            at1 = new List<Location>()
            {
                new Location(6,3),
                new Location(5,3),
                new Location(5,4),
                new Location(6,5),
                new Location(7,4),
                new Location(7,3)
            }.OrderBy(compare);
            at2 = new List<Location>()
            {
                new Location(6,2),
                new Location(5,2),
                new Location(4,3),
                new Location(4,4),
                new Location(4,5),
                new Location(5,5),
                new Location(6,6),
                new Location(7,5),
                new Location(8,5),
                new Location(8,4),
                new Location(8,3),
                new Location(7,2)
            }.OrderBy(compare);
            at3 = new List<Location>()
            {
                new Location(6,1),
                new Location(5,1),
                new Location(4,2),
                new Location(3,2),
                new Location(3,3),
                new Location(3,4),
                new Location(3,5),
                new Location(4,6),
                new Location(5,6),
                new Location(6,7),
                new Location(7,6),
                new Location(8,6),
                new Location(9,5),
                new Location(9,4),
                new Location(9,3),
                new Location(9,2),
                new Location(8,2),
                new Location(7,1)
            }.OrderBy(compare);

            Assert.Equal(at0, state.LocationsAroundPoint(point, 0).OrderBy(compare));
            Assert.Equal(at1, state.LocationsAroundPoint(point, 1).OrderBy(compare));
            Assert.Equal(at2, state.LocationsAroundPoint(point, 2).OrderBy(compare));
            Assert.Equal(at3, state.LocationsAroundPoint(point, 3).OrderBy(compare));

            Assert.Equal(at0.Concat(at1).OrderBy(compare), state.LocationsAroundPoint(point, 0, 1).OrderBy(compare));
        }

        [Fact()]
        public void ForkTest()
        {
            var state = GetState();
            var fields = state.GetFields();
            var otherState = state.Fork();
            var otherFields = state.GetFields();

            Assert.Equal(fields.Commanders.Select(cmd => cmd.CommanderID), otherFields.Commanders.Select(cmd => cmd.CommanderID));
            Assert.Equal(fields.CurrentCommanderIndex, otherFields.CurrentCommanderIndex);
            Assert.Equal(fields.Terrain.Select(t => t.Location), otherFields.Terrain.Select(t => t.Location));
            Assert.Equal(fields.Height, otherFields.Height);
            Assert.Equal(fields.Units.Select(unit => unit.UnitID), otherFields.Units.Select(unit => unit.UnitID));
            Assert.Equal(fields.Width, otherFields.Width);
        }

        public static BattleGameState.Fields GetFields()
        {
            var height = 2;
            var width = 2;
            var tiles = height * width;
            var terrain = new Terrain[]
            {
                new Terrain(TerrainTypes.City.Instance, new Location(0, 0), null, true, 0),
                new Terrain(TerrainTypes.Plain.Instance, new Location(0, 1)),
                new Terrain(TerrainTypes.Plain.Instance, new Location(1, 0)),
                new Terrain(TerrainTypes.City.Instance, new Location(1, 1), null, true, 1)
            };

            Assert.Equal(tiles, terrain.Length);

            var unitID = 0;
            var units = new Unit[]
            {
                new Unit(unitID++, UnitTypes.Infantry.Instance, new Location(0, 0), 0),
                new Unit(unitID++, UnitTypes.Infantry.Instance, new Location(1, 1), 1)
            };

            var commanderID = 0;
            var commanders = new Commander[]
            {
                new Commander(CommanderTypes.BasicCommander.Instance, commanderID++),
                new Commander(CommanderTypes.BasicCommander.Instance, commanderID++)
            };

            return new BattleGameState.Fields(height, width, terrain, units, commanders, new Dictionary<string, object>(0), 0);
        }

        public static BattleGameState GetState()
        {
            var state = new BattleGameState();
            state.StartGame(GetFields());
            return state;
        }

        public static void CrossCheckFieldsWithState(BattleGameState.Fields fields, BattleGameState state)
        {
            Assert.Equal(fields.Commanders, state.Commanders.Values);

            foreach (var kp in state.Commanders)
            {
                Assert.Equal(kp.Value.CommanderID, kp.Key);
            }

            Assert.Equal(fields.CurrentCommanderIndex, state.CurrentCommanderIndex);
            Assert.Equal(fields.Height, state.Height);
            Assert.Equal(fields.Terrain.Select(t => t.Location), state.Terrain.Select(t => t.Location));
            Assert.Equal(fields.Units, state.Units.Values);

            foreach (var kp in state.Units)
            {
                Assert.Equal(kp.Value.UnitID, kp.Key);
            }

            Assert.Equal(fields.Width, state.Width);
        }

        public static void CrossCheckFields(BattleGameState.Fields a, BattleGameState.Fields b)
        {
            Assert.Equal(a.Commanders, b.Commanders);
            Assert.Equal(a.CurrentCommanderIndex, b.CurrentCommanderIndex);
            Assert.Equal(a.Terrain, b.Terrain);
            Assert.Equal(a.Height, b.Height);
            Assert.Equal(a.Units, b.Units);
            Assert.Equal(a.Width, b.Width);
        }
    }
}