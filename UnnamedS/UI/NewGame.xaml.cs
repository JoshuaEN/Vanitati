using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    /// Interaction logic for NewGame.xaml
    /// </summary>
    public partial class NewGame : Page
    {
        private Game.BattleGameState.Fields LoadedFields { get; set; }
        private Saving.Wrappers.BaseWrapper.SaveType? SaveType { get; set; }
        private ushort? Port { get; set; }
        private string DisplayName { get; set; }
        private bool VictoryPointLimitEnabled { get; set; }
        private int VictoryPointLimit { get; set; }
        private bool VictoryPointGapEnabled { get; set; }
        private int VictoryPointGap { get; set; }

        public NewGame()
        {
            InitializeComponent();

            optPort.Text = Globals.DEFAULT_PORT.ToString();

            UpdateWrapper(null);
        }

        private void SelectMapButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wrapper = Saving.BattleSaving.LoadBattleGameState(null);
                LoadedFields = wrapper?.Fields;
                SaveType = wrapper?.Type;
                UpdateWrapper(wrapper);
            }
            catch (Exception ex)
            {
                Resource.MainWindow.ShowMessage(ex);
            }
        }

        private void UpdateWrapper(Saving.Wrappers.BaseWrapper wrapper)
        {

            if (wrapper == null)
            {
                msgNoStateSelected.Visibility = Visibility.Visible;
                return;
            }

            msgNoStateSelected.Visibility = Visibility.Collapsed;

            var row = 0;

            
            nooptMaxCommanders.Text = wrapper.Fields.Commanders.Length.ToString();

            object pointsGapObj;
            if (wrapper.Fields.Values.TryGetValue("VictoryPointGap", out pointsGapObj))
                optVictoryPointsGap.Text = ((int)pointsGapObj).ToString();
            else
                optVictoryPointsGap.Text = "20";

            object pointsGapEnabledObj;
            if (wrapper.Fields.Values.TryGetValue("VictoryPointGapEnabled", out pointsGapEnabledObj))
                optEnableVictoryPointsGap.IsChecked = (bool)pointsGapEnabledObj;
            else
                optEnableVictoryPointsGap.IsChecked = true;

            object pointsLimitEnabledObj;
            if (wrapper.Fields.Values.TryGetValue("VictoryPointLimitEnabled", out pointsLimitEnabledObj))
                optEnableVictoryPointsMaximum.IsChecked = (bool)pointsLimitEnabledObj;
            else
                optEnableVictoryPointsMaximum.IsChecked = true;

            object pointsLimitObj;
            if (wrapper.Fields.Values.TryGetValue("VictoryPointLimit", out pointsLimitObj))
                optVictoryPointsMaximum.Text = ((int)pointsLimitObj).ToString();
            else
                optVictoryPointsMaximum.Text = "50";
        }

        private void AddRow(Grid grid, string labelText, object valueText, ref int row)
        {
            grid.RowDefinitions.Add(new RowDefinition());

            var label = new Label() { Content = labelText };
            var value = new Label() { Content =  valueText };

            Grid.SetColumn(value, 1);
            Grid.SetRow(label, row);
            Grid.SetRow(value, row);
            row++;

            grid.Children.Add(label);
            grid.Children.Add(value);
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {

            if (Resource.ShowValidationErrors(Validate()))
                return;

            LoadedFields.Values["VictoryPointLimitEnabled"] = VictoryPointLimitEnabled;
            LoadedFields.Values["VictoryPointGapEnabled"] = VictoryPointGapEnabled;
            LoadedFields.Values["VictoryPointLimit"] = VictoryPointLimit;
            LoadedFields.Values["VictoryPointGap"] = VictoryPointGap;

            var view = new BattleViewV2();
            try
            {
                Resource.MainWindow.SetContent(view);
                Game.GameLogic logic = null;
                if (optLocalGame.IsChecked == true)
                {
                    var ourUser = new Game.User(0, optName.Text, true);

                    logic = new Game.LocalGameLogic();
                    view.LoadLocalGame(logic as Game.LocalGameLogic, new List<Game.User>() { ourUser }, ourUser);
                }
                else if (optNetworkedGame.IsChecked == true)
                {
                    var server = new Network.Server(new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Port.Value));
                    Resource.MainWindow.Server = server;
                    server.Listen();
                    logic = new Game.NetworkedGameLogic(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), Port.Value), DisplayName, new List<Game.IUserLogic>() { view });

                    view.LoadNetworkedGame(logic as Game.NetworkedGameLogic);
                }
                else
                {
                    Contract.Assert(false);
                }

                if (SaveType == Saving.Wrappers.BaseWrapper.SaveType.Map)
                    logic.StartGame(LoadedFields, Game.BattleGameState.StartMode.NewGame);
                else if (SaveType == Saving.Wrappers.BaseWrapper.SaveType.SaveGame)
                    logic.StartGame(LoadedFields, Game.BattleGameState.StartMode.LoadedSaveGame);
                else
                    Contract.Assert(false);

            }
            catch (Exception ex)
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
            {
                if(LoadedFields == null)
                {
                    errors.Add("An Initial State must be selected.");
                }
            }
            {
                if (optEnableVictoryPointsMaximum.IsChecked == true)
                {
                    int pointsMax;
                    if (int.TryParse(optVictoryPointsMaximum.Text, out pointsMax) == false || pointsMax < 0)
                    {
                        errors.Add($"Victory Points Limit must be a valid number of 0 or more, and less than {int.MaxValue}");
                    }
                    else
                    {
                        VictoryPointLimitEnabled = true;
                        VictoryPointLimit = pointsMax;
                    }

                }
                else
                {
                    VictoryPointLimitEnabled = false;
                    VictoryPointLimit = 50;
                }
            }
            {
                if(optEnableVictoryPointsGap.IsChecked == true)
                {
                    int pointsGap;
                    if(int.TryParse(optVictoryPointsGap.Text, out pointsGap) == false || pointsGap < 0)
                    {
                        errors.Add($"Victory Points Gap must be a valid number of 0 or more, and less than {int.MaxValue}");
                    }
                    else
                    {
                        VictoryPointGapEnabled = true;
                        VictoryPointGap = pointsGap;
                    }

                }
                else
                {
                    VictoryPointGapEnabled = false;
                    VictoryPointGap = 20;
                }
            }

            return errors;
        }

    }
}
