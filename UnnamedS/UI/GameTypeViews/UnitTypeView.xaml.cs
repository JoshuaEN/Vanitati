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
    /// Interaction logic for UnitTypeView.xaml
    /// </summary>
    public partial class UnitTypeView : UserControl
    {
        private Game.UnitType _unitType;
        public Game.UnitType UnitType
        {
            get { return _unitType; }
            set
            {
                _unitType = value;
                Update();
            }
        }

        public UnitTypeView()
        {
            InitializeComponent();
        }

        private void Update()
        {
            lblHeader.Content = Globals.GetResource(UnitType.Key);
            lblDesc.Content = Globals.GetResource($"{UnitType.Key}_desc");

            lblMovementType.Content = Globals.GetResource(UnitType.MovementType.Key);

            lblMaxHealth.Content = UnitType.MaxHealth;
            lblArmor.Content = UnitType.MaxArmor;
            lblConcealment.Content = $"{UnitType.Concealment / 100.0:0%;-0%}";
            lblBuildCost.Content = $"{UnitType.BuildCost:C0}";

            lblMaxActions.Content = UnitType.MaxActions;
            lblMaxMovement.Content = UnitType.MaxMovement;

            gridSupplies.Children.Clear();
            gridSupplies.RowDefinitions.Clear();

            {
                var row = 0;

                Resource.GenerateRow(gridSupplies, ref row, "", "Max", "Per-Movement", "Per-Turn");

                foreach(var kp in UnitType.SupplyLimits)
                {
                    var supply = kp.Key;
                    var max = kp.Value;

                    int perMovement;
                    int perTurn;

                    if (UnitType.MovementSupplyUsage.TryGetValue(supply, out perMovement) == false)
                        perMovement = 0;

                    if (UnitType.TurnSupplyUsage.TryGetValue(supply, out perTurn) == false)
                        perTurn = 0;

                    Resource.GenerateRow(gridSupplies, ref row, Globals.GetResource(supply.Key), max, perMovement, perTurn);
                }
            }

            {
                actionList.Children.Clear();

                foreach(var action in UnitType.Actions.Where(a => a.CanUserTrigger))
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

        private void lblMovementType_Click(object sender, RoutedEventArgs e)
        {
            Help.HelpView.Instance.Display(UnitType.MovementType);
        }
    }
}
