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
    /// Interaction logic for PropView.xaml
    /// </summary>
    public partial class PropView : UserControl
    {
        private Game.Properties.IPropertyContainer _propertySource;
        public Game.Properties.IPropertyContainer PropertySource
        {
            get { return _propertySource; }
            set
            {
                _propertySource = value;
                Update();
            }
        }

        public PropView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            propertyGrid.Children.Clear();
            propertyGrid.RowDefinitions.Clear();

            if (PropertySource == null)
                return;

            foreach (var v in PropertySource.GetProperties())
            {
                propertyGrid.RowDefinitions.Add(new RowDefinition());

                var label = new Label();
                label.Content = v.Key;
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, propertyGrid.RowDefinitions.Count - 1);
                propertyGrid.Children.Add(label);

                if (v.Value != null)
                {

                    if (v.Value is System.Collections.IDictionary)
                    {
                        foreach (System.Collections.DictionaryEntry kp in (v.Value as System.Collections.IDictionary))
                        {
                            propertyGrid.Children.Add(GetLabelFor(string.Format("{0},{1}", kp.Key, kp.Value)));
                            propertyGrid.RowDefinitions.Add(new RowDefinition());
                        }
                    }
                    else
                    {
                        propertyGrid.Children.Add(GetLabelFor(v.Value));
                    }
                }
                else
                {
                    propertyGrid.Children.Add(GetLabelFor(v.Value));
                }
            }
        }

        private Label GetLabelFor(object content)
        {
            var label = new Label();
            label.HorizontalContentAlignment = HorizontalAlignment.Left;
            label.Content = content;
            Grid.SetColumn(label, 1);
            Grid.SetRow(label, propertyGrid.RowDefinitions.Count - 1);
            return label;
        }
    }
}
