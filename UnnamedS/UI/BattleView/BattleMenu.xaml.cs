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
    /// Interaction logic for BattleMenu.xaml
    /// </summary>
    public partial class BattleMenu : UserControl
    {
        public BattleMenu()
        {
            InitializeComponent();
        }

        public event EventHandler SaveGame;
        public event EventHandler ExitToMenu;
        public event EventHandler ExitToDesktop;
        public event EventHandler ReturnToBattle;
        public event EventHandler CommanderAssignment;

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            SaveGame?.Invoke(this, new EventArgs());
        }

        private void ExitToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ExitToMenu?.Invoke(this, new EventArgs());
        }

        private void ExitToDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            ExitToDesktop?.Invoke(this, new EventArgs());
        }

        private void ReturnToBattleButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToBattle?.Invoke(this, new EventArgs());
        }

        private void CommanderAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            CommanderAssignment?.Invoke(this, new EventArgs());
        }
    }
}
