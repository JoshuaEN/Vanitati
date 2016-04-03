using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for NetworkLogViewer.xaml
    /// </summary>
    public partial class NetworkLogViewer : Window
    {
        private System.Windows.Forms.PropertyGrid propContainer = new System.Windows.Forms.PropertyGrid();
        public NetworkLogViewer()
        { 
            InitializeComponent();

            propContainerHost.Child = propContainer;
        }

        public void AddSource(string header, Network.IClientNotifier notifySource)
        {
            var view = new LogView(notifySource);
            view.EntrySelected += View_EntrySelected;
            var tab = new TabItem();
            tab.Content = view;
            tab.Header = header;

            tabControl.Items.Add(tab);
        }

        private void View_EntrySelected(object sender, LogView.EntrySelectedEventArgs e)
        {
            propContainer.SelectedObject = e.SelectedEntry;
        }
    }
}
