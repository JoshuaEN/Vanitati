using System;
using System.Collections.Generic;
using System.Data;
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

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for DamageTable.xaml
    /// </summary>
    public partial class DamageTable : Page
    {
        public DamageTable()
        {
            InitializeComponent();

            foreach(var terrainType in Game.TerrainType.TYPES.Values)
            {
                comboBoxSourceTerrain.Items.Add(terrainType);
                comboBoxTargetTerrain.Items.Add(terrainType);
            }

            comboBoxSourceTerrain.SelectedItem = Game.TerrainTypes.Plain.Instance;
            comboBoxTargetTerrain.SelectedItem = Game.TerrainTypes.Plain.Instance;

            comboBoxSourceTerrain.SelectionChanged += comboBoxTerrain_SelectionChanged;
            comboBoxTargetTerrain.SelectionChanged += comboBoxTerrain_SelectionChanged;

            dataGrid.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;

            UpdateTable();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if((e.Column.Header as string).EndsWith("_MODIFIERS"))
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var cell = e.AddedCells.FirstOrDefault();

            if(cell != null)
            {
                try
                {
                    var column = cell.Column.Header;


                    var modifiers = (IReadOnlyList<ActionType.Modifier>)(cell.Item as DataRowView)[column + "_MODIFIERS"];

                    actionDetails.Modifiers = modifiers;
                }
                catch
                {
                    actionDetails.Modifiers = new List<ActionType.Modifier>(0);
                }
            }
        }

        public void UpdateTable()
        {
            var tileWith = (TerrainType)comboBoxTargetTerrain.SelectedItem;
            var logic = new LocalGameLogic();

            int height = 100, width = 100;
            Location sourceLocation = new Location(width / 2, height / 2);

            Terrain[] terrain = new Terrain[height * width];

            for(var h = 0; h < height; h++)
            {
                for(var w = 0; w < width; w++)
                {
                    var i = h + height * w;

                    var type = tileWith;

                    if (h == sourceLocation.Y && w == sourceLocation.X)
                        type = (TerrainType)comboBoxSourceTerrain.SelectedItem;

                    terrain[i] = new Terrain(type, new Location(w, h));
                }
            }

            logic.StartGame(new BattleGameState.Fields(height, width, terrain, new Unit[0], new Commander[]
            {
                new Commander(Game.CommanderTypes.BasicCommander.Instance, 0, 0),
                new Commander(Game.CommanderTypes.BasicCommander.Instance, 1, 0)
            }, new Dictionary<string, object>(), -1));

            DataTable dt = new DataTable();
            dt.Columns.Add("source_unit");
            dt.Columns.Add("source_unit_action");

            var foreachOrder = new List<UnitType>();

            foreach (var rndUnitType in UnitType.TYPES.Values)
            {
                dt.Columns.Add(Globals.GetResource(rndUnitType.Key));
                dt.Columns.Add(Globals.GetResource(rndUnitType.Key) + "_MODIFIERS", typeof(object));
                foreachOrder.Add(rndUnitType);
            }

            int sourceUnitID = 0, targetUnitID = 1;

            foreach (var sourceUnitType in foreachOrder.ToArray())
            {
                foreach (var actionGeneric in sourceUnitType.Actions.Where(a => a is Game.ActionTypes.ForUnits.AttackBase))
                {
                    var action = (Game.ActionTypes.ForUnits.AttackBase)actionGeneric;

                    var row = dt.NewRow();
                    row["source_unit"] = Globals.GetResource(sourceUnitType.Key);
                    row["source_unit_action"] = Globals.GetResource(action.Key);

                    logic.InternalState.AddUnit(new Unit(sourceUnitID, sourceUnitType, sourceLocation, sourceUnitID, null, null, null, null, sourceUnitType.MaxMovement, sourceUnitType.MaxActions));

                    var targetLocation = logic.State.LocationsAroundPoint(sourceLocation, action.MinimumRange, action.MaximumRange).First();

                    var context = new Game.Action.ActionContext(
                        sourceUnitID, Game.ActionTypes.UnitAction.ActionTriggers.AttackRetaliation, 
                        new Game.Action.UnitContext(sourceLocation), new Game.Action.UnitContext(targetLocation));
                    var specialContext = new Game.Action.UnitTargetTileContext(logic.State, context);

                    foreach (var targetUnitType in foreachOrder.ToArray())
                    {
                        logic.InternalState.AddUnit(new Unit(targetUnitID, targetUnitType, targetLocation, targetUnitID));

                        string result = null;

                        if(action.CanPerformOn(logic.State, context))
                        {
                            var changes = action.PerformOn(logic.State, context);

                            foreach(var change in changes)
                            {
                                if(change is Game.StateChanges.UnitStateChange)
                                {
                                    var unitChange = (change as Game.StateChanges.UnitStateChange);

                                    if (unitChange.UnitID == targetUnitID)
                                    {
                                        if (unitChange.UpdatedProperties.ContainsKey("Health"))
                                        {
                                            result = (targetUnitType.MaxHealth - (int)unitChange.UpdatedProperties["Health"]).ToString();
                                        }
                                    }
                                }
                            }

                            row[Globals.GetResource(targetUnitType.Key) + "_MODIFIERS"] = action.Modifiers(logic.State, context);
                        }
                        else
                        {
                            result = "";
                        }

                        row[Globals.GetResource(targetUnitType.Key)] = result?.ToString() ?? "-";

                        logic.InternalState.DeleteUnit(targetUnitID);
                    }

                    dt.Rows.Add(row);

                    logic.InternalState.DeleteUnit(sourceUnitID);
                }
            }

            dataGrid.AutoGenerateColumns = true;
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void comboBoxTerrain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTable();
        }
    }
}
