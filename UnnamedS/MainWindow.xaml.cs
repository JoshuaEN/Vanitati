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
using System.Net;
using System.Net.Sockets;

namespace UnnamedStrategyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Network.Server sv;
        public NetworkedGameLogic c1;
        public NetworkedGameLogic c2;
        public MainWindow()
        {
            InitializeComponent();

            {
                object v;

                v = SupplyType.TYPES;
                v = MovementType.TYPES;
                v = ActionType.TYPES;
                v = UnitType.TYPES;

                v = TerrainType.TYPES;

            }

            Content = new UI.BattleView();
            return;

            //var view = new UI.NetworkLogViewer();

            //var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Globals.DEFAULT_PORT);

            //sv = new Network.Server( new TcpListener(endpoint));

            //view.AddSource("Server", sv);

            //sv.Listen();

            //var endpoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), endpoint.Port);
            //var endpoint3 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), endpoint.Port);

            //var cc1 = new Network.Client(endpoint2);
            //var cc2 = new Network.Client(endpoint3);

            //view.AddSource("Client A", cc1);
            //view.AddSource("Client B", cc2);

            //c1 = new Game.NetworkedGameLogic(cc1, new List<IPlayerLogic>());
            //c2 = new Game.NetworkedGameLogic(cc2, new List<IPlayerLogic>());

            //view.Show();


            //var s = Serializers.Serializer.Instance;

            //var m = new Network.MessageWrappers.DoActionsCallWrapper(0, new List<ActionInfo>() { new ActionInfo(Game.ActionTypes.AttackRifle.Instance, new Location(1, 1), new Location(2, 2)) });
            //var res = s.Serialize(m);




            ////var t = new Terrain("terrain_plain");

            ////var res = s.Serialize(t);

            //MessageBox.Show(res);

            //var usres = s.Deserialize<Network.MessageWrappers.MessageWrapper>(res);

            //MessageBox.Show(s.Serialize(usres));

            //MessageBox.Show(UnitType.TYPES.Count.ToString());

            //Console.WriteLine();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
            Environment.Exit(0);
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
