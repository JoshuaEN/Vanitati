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
    /// Interaction logic for JoinGame.xaml
    /// </summary>
    public partial class JoinGame : Page
    {
        private string DisplayName { get; set; }
        private System.Net.IPAddress IPAddress { get; set; }
        private ushort? Port { get; set; }

        public JoinGame()
        {
            InitializeComponent();
            optPort.Text = Globals.DEFAULT_PORT.ToString();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (Resource.ShowValidationErrors(Validate()))
                return;

            var view = new BattleViewV2();

            try
            {
                Resource.MainWindow.SetContent(view);
                var logic = new Game.NetworkedGameLogic(new System.Net.IPEndPoint(IPAddress, Port.Value), DisplayName, new List<Game.IUserLogic>() { view });
                view.LoadNetworkedGame(logic as Game.NetworkedGameLogic);
            }
            catch(Exception ex)
            {
                Resource.MainWindow.SetContent(this);
                Resource.MainWindow.ShowMessage(ex);
                return;
            }

            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new MainMenu());
        }

        private List<string> Validate()
        {
            var errors = new List<string>();

            {
                System.Net.IPAddress ipAddress;

                if (System.Net.IPAddress.TryParse(optIPAddress.Text, out ipAddress) == false)
                {
                    IPAddress = null;

                    errors.Add($"IP Address is not valid");
                }
                else
                {
                    IPAddress = ipAddress;
                }
            }

            {
                ushort result;
                if (ushort.TryParse(optPort.Text, out result) == false)
                {
                    Port = null;

                    errors.Add($"Port must be a number between 0 and {ushort.MaxValue}");
                }
                else
                {
                    Port = result;
                }
            }
            {
                if (string.IsNullOrWhiteSpace(optName.Text))
                {
                    DisplayName = null;

                    errors.Add("Display Name cannot be blank or consist of only spaces.");
                }
                else
                {
                    DisplayName = optName.Text;
                }
            }

            return errors;
        }

    }
}
