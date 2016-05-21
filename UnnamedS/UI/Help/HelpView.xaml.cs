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
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class HelpView : Page
    {
        public Page Back { get; set; }

        private HelpView()
        {
            InitializeComponent();
            Display(new ArticleGettingToBattle());
        }

        public static HelpView Instance { get; } = new HelpView();

        public void Display(Game.BaseType type)
        {
            displayFrame.Content = new DynamicGameTypeDetails() { Type = type };
        }

        public void Display(List<Game.BaseType> types)
        {
            displayFrame.Content = new DynamicGameTypesHelp() { Types = types };
        }

        public void Display(Page page)
        {
            displayFrame.Content = page;
        }

        public void DisplayTerrainTypes()
        {
            Display(Game.TerrainType.TYPES.Values.Cast<Game.BaseType>().ToList());
        }

        public void DisplayUnitTypes()
        {
            Display(Game.UnitType.TYPES.Values.Cast<Game.BaseType>().ToList());
        }

        public void DisplayActionTypes()
        {
            Display(Game.ActionType.TYPES.Values.Where(a => a.CanUserTrigger).Cast<Game.BaseType>().ToList());
        }

        private void GettingToBattleButton_Click(object sender, RoutedEventArgs e)
        {
            Display(new ArticleGettingToBattle());
        }

        private void TerrainButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayTerrainTypes();
        }

        private void UnitsButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayUnitTypes();
        }

        private void ActionsButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayActionTypes();
        }

        private void ControlRundownButton_Click(object sender, RoutedEventArgs e)
        {
            Display(new ArticleInterfaceAndControls());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Resource.MainWindow.SetContent(Back ?? new MainMenu());
        }

        private void GameplayBasicsButton_Click(object sender, RoutedEventArgs e)
        {
            Display(new ArticleGameplayBasics());
        }

        private void TroubleshootingNetworkedGamesButton_Click(object sender, RoutedEventArgs e)
        {
            Display(new ArticleTroubleshootingNetworkGames());
        }
    }
}
