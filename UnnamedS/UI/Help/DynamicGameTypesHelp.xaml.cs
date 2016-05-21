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
    /// Interaction logic for DynamicGameTypesHelp.xaml
    /// </summary>
    public partial class DynamicGameTypesHelp : Page
    {
        private List<Game.BaseType> _types;
        public List<Game.BaseType> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                UpdateListing();
            }
        }

        public DynamicGameTypesHelp()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UpdateListing()
        {
            itemList.Children.Clear();

            foreach(var type in Types.OrderBy(t => Globals.GetResource(t.Key)))
            {
                var button = new Button() { Content = Globals.GetResource(type.Key), Tag = type };
                button.Click += TypeButton_Click;
                itemList.Children.Add(button);
            }
        }

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            HelpView.Instance.Display((sender as Button).Tag as Game.BaseType);
        }

    }
}
