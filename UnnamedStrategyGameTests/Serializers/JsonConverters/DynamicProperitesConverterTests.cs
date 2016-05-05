using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGameTests.TestHelpers;
using UnnamedStrategyGame.Game.StateChanges;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class DynamicProperitesConverterTests
    {
        public DynamicProperitesConverterTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void ReadJsonTest()
        {
            var s = new JsonSerializer();

            var unit = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(5, 5), 5);
            SerializerCrossChecks.Check(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location), typeof(UnitStateChange), s);
            var terrain = new Terrain(Game.TerrainTypes.Plain.Instance, new Location(5, 5));
            SerializerCrossChecks.Check(new TerrainStateChange(terrain.Location, terrain.GetWriteableProperties()), typeof(TerrainStateChange), s);
            var commander = new Commander(Game.CommanderTypes.BasicCommander.Instance, 5, 5);
            SerializerCrossChecks.Check(new CommanderStateChange(commander.CommanderID, commander.GetWriteableProperties()), typeof(CommanderStateChange), s);
        }

        [Fact()]
        public void ReadJsonTest_Invalid()
        {
            var s = new JsonSerializer();

            var unit = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(5, 5), 5);
            var unit_dffID = new Unit(4, Game.UnitTypes.Infantry.Instance, new Location(5, 5), 5);
            var unit_dffType = new Unit(5, Game.UnitTypes.ReconCar.Instance, new Location(5, 5), 5);
            var unit_diffLocationX = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(4, 5), 5);
            var unit_diffLocationY = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(5, 4), 5);
            var unit_diffCmd = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(5, 5), 4);
            SerializerCrossChecks.CheckNot(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location), 
                new UnitStateChange(unit_dffID.UnitID, unit_dffID.GetWriteableProperties(), unit_dffID.Location), typeof(UnitStateChange), s);
            SerializerCrossChecks.CheckNot(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location),
                new UnitStateChange(unit_dffType.UnitID, unit_dffType.GetWriteableProperties(), unit_dffType.Location), typeof(UnitStateChange), s);
            SerializerCrossChecks.CheckNot(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location),
                new UnitStateChange(unit_diffLocationX.UnitID, unit_diffLocationX.GetWriteableProperties(), unit_diffLocationX.Location), typeof(UnitStateChange), s);
            SerializerCrossChecks.CheckNot(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location),
                new UnitStateChange(unit_diffLocationY.UnitID, unit_diffLocationY.GetWriteableProperties(), unit_diffLocationY.Location), typeof(UnitStateChange), s);
            SerializerCrossChecks.CheckNot(new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location),
                new UnitStateChange(unit_diffCmd.UnitID, unit_diffCmd.GetWriteableProperties(), unit_diffCmd.Location), typeof(UnitStateChange), s);

            var terrain = new Terrain(Game.TerrainTypes.Plain.Instance, new Location(5, 5));
            var terrain_diffType = new Terrain(Game.TerrainTypes.City.Instance, new Location(5, 5));
            var terrain_diffLocationX = new Terrain(Game.TerrainTypes.Plain.Instance, new Location(4, 5));
            var terrain_diffLocationY = new Terrain(Game.TerrainTypes.Plain.Instance, new Location(5, 4));
            SerializerCrossChecks.CheckNot(new TerrainStateChange(terrain.Location, terrain.GetWriteableProperties()), 
                new TerrainStateChange(terrain_diffType.Location, terrain_diffType.GetWriteableProperties()), typeof(TerrainStateChange), s);
            SerializerCrossChecks.CheckNot(new TerrainStateChange(terrain.Location, terrain.GetWriteableProperties()),
                new TerrainStateChange(terrain_diffLocationX.Location, terrain_diffLocationX.GetWriteableProperties()), typeof(TerrainStateChange), s);
            SerializerCrossChecks.CheckNot(new TerrainStateChange(terrain.Location, terrain.GetWriteableProperties()),
                new TerrainStateChange(terrain_diffLocationY.Location, terrain_diffLocationY.GetWriteableProperties()), typeof(TerrainStateChange), s);

            var commander = new Commander(Game.CommanderTypes.BasicCommander.Instance, 5, 5);
            //var commander_diffType = new Commander(FakeCommander.Instance, 5, 5);
            var commander_diffID = new Commander(Game.CommanderTypes.BasicCommander.Instance, 4, 5);
            var commander_diffCredits = new Commander(Game.CommanderTypes.BasicCommander.Instance, 5, 4);
            //SerializerCrossChecks.CheckNot(new CommanderStateChange(commander.CommanderID, commander.GetWriteableProperties()),
            //    new CommanderStateChange(commander_diffType.CommanderID, commander_diffType.GetWriteableProperties()), typeof(CommanderStateChange), s);
            SerializerCrossChecks.CheckNot(new CommanderStateChange(commander.CommanderID, commander.GetWriteableProperties()),
                new CommanderStateChange(commander_diffID.CommanderID, commander_diffID.GetWriteableProperties()), typeof(CommanderStateChange), s);
            SerializerCrossChecks.CheckNot(new CommanderStateChange(commander.CommanderID, commander.GetWriteableProperties()),
                new CommanderStateChange(commander_diffCredits.CommanderID, commander_diffCredits.GetWriteableProperties()), typeof(CommanderStateChange), s);
        }

        [Fact()]
        public void ReadJsonTest_Tampered()
        {
            var s = new JsonSerializer();

            var unit = new Unit(5, Game.UnitTypes.Infantry.Instance, new Location(5, 5), 5);
            var change = new UnitStateChange(unit.UnitID, unit.GetWriteableProperties(), unit.Location);
            var sres = s.Serialize(change);
            var tsres = sres.Replace("Movement", "Hectares");

            Assert.Throws<ArgumentException>(() =>
            {
                s.Deserialize<UnitStateChange>(tsres);
            });
        }

        internal class FakeCommander : CommanderType
        {
            private FakeCommander() : base("fake_commander") { }
            public static FakeCommander Instance { get; } = new FakeCommander();
        }
    }
}