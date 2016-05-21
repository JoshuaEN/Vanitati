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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        private Page Back { get; }
        public SettingsView(Page back)
        {
            InitializeComponent();
            Back = back;
            Load();
        }

        public void Load()
        {
            optDisplayMode.ItemsSource = Enum.GetValues(typeof(Settings.Settings.WindowDisplayMode));
            
        }

        private void optDisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(Back);
        }
    }
}
