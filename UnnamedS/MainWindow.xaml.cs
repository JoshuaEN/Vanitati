using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            object v;

            v = ActionType.TYPES;
            v = MovementType.TYPES;
            v = SupplyType.TYPES;
            v = UnitType.TYPES;

            v = TerrainType.TYPES;

            BaseType.BuildAttributeDefinitionsListing();

            var s = Serializers.Serializer.Instance;

            var m = new Network.MessageWrappers.DoActionsCallWrapper(new Location(1, 1), new Location(2, 2), new List<ActionType>() { Game.ActionTypes.AttackRifle.Instance });
            var res = s.Serialize(m);




            //var t = new Terrain("terrain_plain");

            //var res = s.Serialize(t);

            MessageBox.Show(res);

            var usres = s.Deserialize<Network.MessageWrappers.MessageWrapper>(res);

            MessageBox.Show(s.Serialize(usres));

            MessageBox.Show(UnitType.TYPES.Count.ToString());

            Console.WriteLine();
        }
    }

    public class TestPropertyGet
    {
        public string Prop { get; }
        public TestPropertyGet(string str)
        {
            Prop = str;
        }
    }
}
