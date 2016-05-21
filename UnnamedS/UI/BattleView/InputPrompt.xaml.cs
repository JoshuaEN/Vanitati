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
    /// Interaction logic for InputPrompt.xaml
    /// </summary>
    public partial class InputPrompt : UserControl
    {
        private System.Collections.IEnumerable _options;
        public System.Collections.IEnumerable Options
        {
            get { return _options; }
            set { _options = value; UpdateOptionList(); }
        }

        public InputPrompt()
        {
            InitializeComponent();
        }

        public event EventHandler<OptionSelectedEventArgs> OptionSelected;

        private void OnOptionSelected(object selectedOption)
        {
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs(selectedOption));
        }
        
        private void UpdateOptionList()
        {
            stackPanel.Children.Clear();

            foreach(var option in Options)
            {
                var button = new Button();
                button.Tag = option;
                button.Click += Button_Click;

                if (option is string)
                    button.Content = Globals.GetResource((string)option);
                else
                    button.Content = Globals.GetResource(option.ToString());

                stackPanel.Children.Add(button);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnOptionSelected((sender as Button).Tag);
        }

        public class OptionSelectedEventArgs : EventArgs
        {
            public object SelectedOption { get; }

            public OptionSelectedEventArgs(object selectedOption)
            {
                SelectedOption = selectedOption;
            }
        }
    }
}
