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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new NewGame());
        }

        private void JoinNetworkedGameButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new JoinGame());
        }

        private void ExitToDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoadMapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new MapEditor());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new SettingsView(this));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(Help.HelpView.Instance);
        }

        private void DamageTableButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(new DamageTable());
        }
    }
}
