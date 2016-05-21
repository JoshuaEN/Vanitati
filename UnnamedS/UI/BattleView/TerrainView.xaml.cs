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
    /// Interaction logic for TerrainView.xaml
    /// </summary>
    public partial class TerrainView : UserControl
    {
        private Terrain _terrain;
        public Terrain Terrain
        {
            get { return _terrain; }
            set
            {
                _terrain = value;
                Update();
            }
        }

        public BattleViewV2 View { get; set; }

        public TerrainView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if(Terrain == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            Visibility = Visibility.Visible;


            lblHeader.Content = Globals.GetResource(Terrain.TerrainType.Key);

            if (Terrain.TerrainType.DigInCap > 0)
                lblDigIn.Content = $"{Terrain.DigIn} / {Terrain.TerrainType.DigInCap}";
            else
                lblDigIn.Content = "Cannot be Fortified";

            if (Terrain.TerrainType.CanBePillage)
                lblHealth.Content = $"{Terrain.Health} / {Terrain.TerrainType.MaxHealth}";
            else
                lblHealth.Content = "Cannot be Pillaged";

            {
                string text; Brush background;
                Resource.GetCommanderIDNameAndColorForTerrain(View.Logic, Terrain, out text, out background);

                borderHeader.BorderBrush = background;
            }
            

            {
                gridCaptureProgress.Children.Clear();
                gridCaptureProgress.RowDefinitions.Clear();

                var row = 0;
                foreach (var progress in Terrain.CaptureProgress)
                {
                    gridCaptureProgress.RowDefinitions.Add(new RowDefinition());

                    string text; Brush background;
                    Resource.GetCommanderIDNameAndColor(View.Logic, progress.Key, out text, out background);

                    var bar = new ProgressBar() { Maximum = Terrain.TerrainType.MaxCapturePoints, Value = progress.Value, Foreground = background };
                    var txt = new Label() { Content = new TextBlock() { Text = $"{text} ({progress.Value} of {Terrain.TerrainType.MaxCapturePoints})" } };

                    Grid.SetRow(bar, row);
                    Grid.SetRow(txt, row);

                    gridCaptureProgress.Children.Add(bar);
                    gridCaptureProgress.Children.Add(txt);

                    row += 1;
                }
            }
        }
    }
}
