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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public ObservableCollection<EventArgs> Items { get; } = new ObservableCollection<EventArgs>();

        public event EventHandler<EntrySelectedEventArgs> EntrySelected;

        public LogView(Network.IClientNotifier notifySource)
        {
            notifySource.Exception += NotifySource_Exception;
            notifySource.Disconnected += NotifySource_Disconnected;
            notifySource.MessageReceived += NotifySource_MessageReceived;
            InitializeComponent();
        }

        private void NotifySource_MessageReceived(object sender, Network.MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Items.Add(e);
            });
        }

        private void NotifySource_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Items.Add(e);
            });
        }

        private void NotifySource_Exception(object sender, Network.ExceptionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Items.Add(e);
            });
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var handler = EntrySelected;
            if(null != handler)
            {
                handler(this, new EntrySelectedEventArgs(listBox.SelectedItem));
            }
        }

        public class EntrySelectedEventArgs
        {
            public object SelectedEntry { get; }

            public EntrySelectedEventArgs(object selectedEntry)
            {
                SelectedEntry = selectedEntry;
            }
        }
    }
}
