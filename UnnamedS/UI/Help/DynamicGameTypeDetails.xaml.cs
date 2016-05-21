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

namespace UnnamedStrategyGame.UI.Help
{
    /// <summary>
    /// Interaction logic for DynamicGameTypeDetails.xaml
    /// </summary>
    public partial class DynamicGameTypeDetails : Page
    {
        private Game.BaseType _type { get; set; }
        public Game.BaseType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                DisplayGameType();
            }
        }

        public DynamicGameTypeDetails()
        {
            InitializeComponent();
        }

        private void DisplayGameType()
        {
            var unitType = Type as Game.UnitType;
            var terrainType = Type as Game.TerrainType;
            var commanderType = Type as Game.CommanderType;
            var movementType = Type as Game.MovementType;
            var actionType = Type as Game.ActionType;

            if (unitType != null)
                mainGrid.Children.Add(new GameTypeViews.UnitTypeView() { UnitType = unitType });
            else if (terrainType != null)
                mainGrid.Children.Add(new GameTypeViews.TerrainTypeView() { TerrainType = terrainType });
            else if (actionType != null)
                mainGrid.Children.Add(new GameTypeViews.ActionTypeView() { ActionType = actionType });
            else if (movementType != null)
                mainGrid.Children.Add(new GameTypeViews.MovementTypeView() { MovementType = movementType });
            else
                mainGrid.Children.Add(new Label() { Content = $"Game Type of {Type.GetType()} is not supported." });
        }
    }
}
