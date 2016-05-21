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

namespace UnnamedStrategyGame.UI.GameTypeViews
{
    /// <summary>
    /// Interaction logic for TerrainTypeView.xaml
    /// </summary>
    public partial class TerrainTypeView : UserControl
    {
        private Game.TerrainType _terrainType;
        public Game.TerrainType TerrainType
        {
            get { return _terrainType; }
            set
            {
                _terrainType = value;
                Update();
            }
        }

        public TerrainTypeView()
        {
            InitializeComponent();
        }

        private void Update()
        {
            lblHeader.Content = Globals.GetResource(TerrainType.Key);
            lblDesc.Content = Globals.GetResource($"{TerrainType.Key}_desc");

            lblMaxHealth.Content = TerrainType.MaxHealth;
            lblConcealmentModifier.Content = $"{TerrainType.ConcealmentModifier / 100.0:0%;-0%}";
            lblClassification.Content = TerrainType.Classification;
            lblDifficultly.Content = TerrainType.Difficultly;
            lblHeight.Content = TerrainType.Height;
            lblMaxCapturePoints.Content = TerrainType.MaxCapturePoints;
            lblMaxDigIn.Content = TerrainType.DigInCap;

            gridRepairs.Children.Clear();
            gridRepairs.RowDefinitions.Clear();

            if(TerrainType.CanRepair)
            {
                var row = 0;

                Resource.GenerateRow(gridRepairs, ref row, "Unit Movement Type", "HP Repaired (if owner)");

                foreach (var kp in TerrainType.RepairsPerTurn)
                {
                    var movementType = kp.Key;
                    var hpRepaired = kp.Value;

                    Resource.GenerateRow(gridRepairs, ref row, Globals.GetResource(movementType.Key), hpRepaired);
                }

                if(TerrainType.RepairsPerTurn.Count == 0)
                {
                    Resource.GenerateRow(gridRepairs, ref row, "(none)");
                }
            }
            else
            {
                var row = 0;
                Resource.GenerateRow(gridRepairs, ref row, "Cannot Repair Units");
            }

            gridResupplies.Children.Clear();
            gridResupplies.RowDefinitions.Clear();

            if(TerrainType.CanSupply)
            {
                var row = 0;

                Resource.GenerateRow(gridResupplies, ref row, "Supply", "Amount Replenished (if owner)");

                foreach (var kp in TerrainType.ResuppliesPerTurn)
                {
                    var supplyType = kp.Key;
                    var suppliesReplinished = kp.Value;

                    Resource.GenerateRow(gridResupplies, ref row, Globals.GetResource(supplyType.Key), suppliesReplinished);
                }

                if (TerrainType.ResuppliesPerTurn.Count == 0)
                {
                    Resource.GenerateRow(gridResupplies, ref row, "(none)");
                }
            }
            else
            {
                var row = 0;
                Resource.GenerateRow(gridResupplies, ref row, "Cannot Resupply Units");
            }

            {
                actionList.Children.Clear();

                foreach (var action in TerrainType.Actions.Where(a => a.CanUserTrigger))
                {
                    var button = new Button() { Content = Globals.GetResource(action.Key), Tag = action };
                    button.Click += ActionButton_Click;

                    actionList.Children.Add(button);
                }
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            Help.HelpView.Instance.Display((sender as Button).Tag as Game.ActionType);
        }
    }
}
