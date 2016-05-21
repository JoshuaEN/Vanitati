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

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for MapEditorPlacementModeEditor.xaml
    /// </summary>
    public partial class MapEditorPlacementModeEditor : UserControl
    {
        private List<int> _validCommanderIDs = new List<int>(0);
        public List<int> ValidCommanderIDs
        {
            get { return _validCommanderIDs; }
            set
            {
                _validCommanderIDs = value;
                UpdateCommanderOptions();
            }
        }

        private int? _commanderID;
        public int? CommanderID
        {
            get { return _commanderID; }
            set
            {
                _commanderID = value;
                Load(Scale);
            }
        }

        private double Scale { get; set; }

        public MapEditorPlacementModeEditor()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void UpdateCommanderOptions()
        {
            commanderChoicesWrapPanel.Children.Clear();


            var label = new Button() { Content = "<None/NPC>", Tag = null, Background = Brushes.Transparent, Padding = new Thickness(8) };
            label.Click += CommanderLabel_Click;
            commanderChoicesWrapPanel.Children.Add(label);

            foreach(var id in ValidCommanderIDs)
            {
                label = new Button() { Content = id, Tag = id, Background = Globals.GetCommanderColor(id), Padding = new Thickness(8) };
                label.Click += CommanderLabel_Click;
                commanderChoicesWrapPanel.Children.Add(label);
            }
        }

        private void CommanderLabel_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CommanderID = (sender as FrameworkElement).Tag as int?;
            }
        }

        public event EventHandler<ModeSelectedEventArgs> ModeSelected;
        public event EventHandler Cancel;

        public void Load(double scale)
        {
            Scale = scale;
            terrainChoicesWrapPanel.Children.Clear();

            foreach(var terrainType in TerrainType.TYPES.Values)
            {
                Commander commander = null;

                if (CommanderID.HasValue && terrainType.CanCapture)
                    commander = new Commander(Game.CommanderTypes.BasicCommander.Instance, CommanderID.Value);

                var stv = new SingleTileView();
                stv.Set(new Terrain(terrainType, SingleTileView.MockBattleGameState.Location, null, commander != null, commander != null ? commander.CommanderID : -1), null, commander, scale);
                stv.MouseDown += Stv_MouseDown;
                terrainChoicesWrapPanel.Children.Add(stv);
            }

            {
                unitChoicesWrapPanel.Children.Clear();
                List<UnitType> unitValues = new List<UnitType>();

                Commander commander = null;
                if (CommanderID.HasValue)
                {
                    unitValues.AddRange(UnitType.TYPES.Values);
                    commander = new Commander(Game.CommanderTypes.BasicCommander.Instance, CommanderID.Value);
                }
                else
                {
                    commander = new Commander(Game.CommanderTypes.BasicCommander.Instance, 0);
                }
                unitValues.Add(MapEditor.RemoveUnitType.Instance);
                 

                foreach (var unitType in unitValues)
                {

                    var stv = new SingleTileView();
                    stv.Set(
                        new Terrain(Tile.Void.Instance, SingleTileView.MockBattleGameState.Location, null),
                        new Unit(0, unitType, SingleTileView.MockBattleGameState.Location, commander.CommanderID),
                        commander,
                        scale);
                    stv.MouseDown += Stv_MouseDown;
                    unitChoicesWrapPanel.Children.Add(stv);
                }
            }

            UpdateChoicesWrapPanels();
        }

        private void UpdateChoicesWrapPanels()
        {
            foreach (var tile in terrainChoicesWrapPanel.Children.Cast<SingleTileView>())
            {
                tile.Update();
            }
            foreach (var tile in unitChoicesWrapPanel.Children.Cast<SingleTileView>())
            {
                tile.Update();
            }
        }

        private void Stv_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var stv = (sender as SingleTileView);
            if (stv.Unit != null)
                ModeSelected?.Invoke(this, new ModeSelectedEventArgs(stv.Unit.UnitType, CommanderID));
            else
                ModeSelected?.Invoke(this, new ModeSelectedEventArgs(stv.Terrain.TerrainType, (stv.Terrain.TerrainType.CanCapture ? CommanderID : null)));
                
        }

        public class ModeSelectedEventArgs : EventArgs
        {
            public TerrainType TerrainType { get; }
            public UnitType UnitType { get; }
            public int? CommanderID { get; }

            public ModeSelectedEventArgs(TerrainType terrainType, int? commanderID)
            {
                TerrainType = terrainType;
                CommanderID = commanderID;
            }

            public ModeSelectedEventArgs(UnitType unitType, int? commanderID)
            {
                UnitType = unitType;
                CommanderID = commanderID;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(this, new EventArgs());
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateChoicesWrapPanels();
            UpdateCommanderOptions();
        }

    }
}
