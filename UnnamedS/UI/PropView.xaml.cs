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
using UnnamedStrategyGame.Game;

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

                var label = new TextBlock();
                label.Text = Globals.GetResource("state_" + v.Key);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Margin = new Thickness(5);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, propertyGrid.RowDefinitions.Count - 1);
                propertyGrid.Children.Add(label);

                if (v.Value != null)
                {

                    if (v.Value is System.Collections.IDictionary)
                    {
                        foreach (System.Collections.DictionaryEntry kp in (v.Value as System.Collections.IDictionary))
                        {
                            propertyGrid.Children.Add(GetLabelFor(string.Format("{0}: {1}", Globals.GetResource(kp.Key?.ToString()), Globals.GetResource(kp.Value?.ToString()))));
                            propertyGrid.RowDefinitions.Add(new RowDefinition());
                        }
                    }
                    else if(v.Value is System.Collections.IEnumerable)
                    {
                        foreach(var val in (v.Value as System.Collections.IEnumerable))
                        {
                            propertyGrid.Children.Add(GetLabelFor(Globals.GetResource(val?.ToString())));
                            propertyGrid.RowDefinitions.Add(new RowDefinition());
                        }
                    }
                    else if(v.Value is ActionInfo)
                    {
                        propertyGrid.Children.Add(GetLabelFor(Globals.GetResource((v.Value as ActionInfo).Type.Key)));
                    }
                    else
                    {
                        propertyGrid.Children.Add(GetLabelFor(Globals.GetResource(v.Value?.ToString())));
                    }
                }
                else
                {
                    propertyGrid.Children.Add(GetLabelFor(Globals.GetResource(v.Value?.ToString())));
                }
            }
        }

        private TextBlock GetLabelFor(object content)
        {
            var label = new TextBlock();
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Text = content?.ToString();
            label.Margin = new Thickness(5);
            Grid.SetColumn(label, 1);
            Grid.SetRow(label, propertyGrid.RowDefinitions.Count - 1);
            return label;
        }
    }
}
