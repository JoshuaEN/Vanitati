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
    /// Interaction logic for MovementTypeView.xaml
    /// </summary>
    public partial class MovementTypeView : UserControl
    {
        private Game.MovementType _movementType;
        public Game.MovementType MovementType
        {
            get { return _movementType; }
            set
            {
                _movementType = value;
                Update();
            }
        }

        public MovementTypeView()
        {
            InitializeComponent();
        }

        private void Update()
        {
            lblHeader.Content = Globals.GetResource(MovementType.Key);
            lblDesc.Content = Globals.GetResource($"{MovementType.Key}_desc");


            gridMovementCost.Children.Clear();
            gridMovementCost.RowDefinitions.Clear();

            {
                var row = 0;

                Resource.GenerateRow(gridMovementCost, ref row, "Terrain", "Cost");

                foreach (var kp in Game.TerrainType.TYPES.Values.Select(t => MovementType.CanTraverse(t) ? new KeyValuePair<Game.TerrainType, int?>(t, MovementType.GetMovementCost(t)) : new KeyValuePair<Game.TerrainType, int?>(t, null)))
                {
                    var terrain = kp.Key;
                    var cost = kp.Value;

                    Resource.GenerateRow(gridMovementCost, ref row, Globals.GetResource(terrain.Key), cost);
                }
            }

            {
                unitList.Children.Clear();

                foreach (var unit in Game.UnitType.TYPES.Values.Where(u => u.MovementType == MovementType))
                {
                    var button = new Button() { Content = Globals.GetResource(unit.Key), Tag = unit };
                    button.Click += UnitButton_Click; ;

                    unitList.Children.Add(button);
                }
            }
        }

        private void UnitButton_Click(object sender, RoutedEventArgs e)
        {
            Help.HelpView.Instance.Display(((sender as Button).Tag as Game.UnitType));
        }
    }
}
