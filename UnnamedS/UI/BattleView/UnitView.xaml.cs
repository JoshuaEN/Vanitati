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

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for UnitView.xaml
    /// </summary>
    public partial class UnitView : UserControl
    {
        private Game.Unit _unit;
        public Game.Unit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                Update();
            }
        }

        public BattleViewV2 View { get; set; }

        public UnitView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if(Unit == null)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            var terrain = View.State.GetTerrain(Unit.Location);

            Visibility = Visibility.Visible;

            lblHeader.Content = Globals.GetResource(Unit.UnitType.Key);
            borderHeader.BorderBrush = Globals.GetCommanderColor(Unit.CommanderID);

            lblHealth.Content = $"{Unit.Health} / {Unit.UnitType.MaxHealth}";
            lblArmor.Content = $"{Unit.Armor}";

            {
                var overall_concealment = Unit.GetEffectiveConcealment(View.State, terrain);
                var terrain_modifer = Unit.GetConcealmentTerrainModifier(View.State, terrain);
                var dig_in_bonus = Unit.GetConcealmentDigInBonus(View.State, terrain);

                lblConcealment.Content = $"{overall_concealment / 100.0:0%;-0%}{terrain_modifer / 100.0: (+0%); (-0%);}{dig_in_bonus / 100.0: (+0%); (-0%);}";
            }

            {
                gridSupplies.Children.Clear();
                gridSupplies.RowDefinitions.Clear();

                {
                    gridSupplies.RowDefinitions.Add(new RowDefinition());

                    var hasLabel = new Label() { Content = "Has" };
                    var maxLabel = new Label() { Content = "Max" };
                    var perMoveLabel = new Label() { Content = "Per\nMove-\nment" };
                    var perTurnLabel = new Label() { Content = "Per\nTurn" };

                    Grid.SetColumn(hasLabel, 2);
                    Grid.SetColumn(perMoveLabel, 3);
                    Grid.SetColumn(perTurnLabel, 4);
                    Grid.SetColumn(maxLabel, 5);

                    gridSupplies.Children.Add(hasLabel);
                    gridSupplies.Children.Add(perMoveLabel);
                    gridSupplies.Children.Add(perTurnLabel);
                    gridSupplies.Children.Add(maxLabel);
                }
                var row = 1;
                foreach (var supply in Unit.Supplies.OrderBy(kp => Globals.GetResource(kp.Key.Key)))
                {
                    gridSupplies.RowDefinitions.Add(new RowDefinition());

                    int perTurnUsage;
                    int perMoveUsage;

                    if (Unit.UnitType.MovementSupplyUsage.TryGetValue(supply.Key, out perMoveUsage) == false)
                        perMoveUsage = 0;
                    if (Unit.UnitType.TurnSupplyUsage.TryGetValue(supply.Key, out perTurnUsage) == false)
                        perTurnUsage = 0;

                    var typeLabel = new Label() { Content = Globals.GetResource(supply.Key.Key) };
                    var hasLabel = new Label() { Content = supply.Value };
                    var maxLabel = new Label() { Content = Unit.UnitType.SupplyLimits[supply.Key] };
                    var perMoveLabel = new Label() { Content = perMoveUsage };
                    var perTurnLabel = new Label() { Content = perTurnUsage };

                    Grid.SetColumn(typeLabel, 1);
                    Grid.SetColumn(hasLabel, 2);
                    Grid.SetColumn(perMoveLabel, 3);
                    Grid.SetColumn(perTurnLabel, 4);
                    Grid.SetColumn(maxLabel, 5);

                    Grid.SetRow(typeLabel, row);
                    Grid.SetRow(hasLabel, row);
                    Grid.SetRow(perMoveLabel, row);
                    Grid.SetRow(perTurnLabel, row);
                    Grid.SetRow(maxLabel, row);

                    gridSupplies.Children.Add(typeLabel);
                    gridSupplies.Children.Add(hasLabel);
                    gridSupplies.Children.Add(perMoveLabel);
                    gridSupplies.Children.Add(perTurnLabel);
                    gridSupplies.Children.Add(maxLabel);

                    row += 1;
                }
            }

            lblMovement.Content = $"{Unit.Movement} / {Unit.UnitType.MaxMovement}";
            lblActions.Content = $"{Unit.Actions} / {Unit.UnitType.MaxActions}";
        }
    }
}
