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
    /// Interaction logic for MapSizeEditor.xaml
    /// </summary>
    public partial class MapSizeEditor : UserControl
    {
        private int _height;
        public int MapHeight
        {
            get { return _height; }
            set
            {
                _height = value;
                optHeight.Text = _height.ToString();
                lblCurrentHeight.Content = _height;
            }
        }

        private int _width;
        public int MapWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                optWidth.Text = _width.ToString();
                lblCurrentWidth.Content = _width;
            }
        }

        public MapSizeEditor()
        {
            InitializeComponent();
        }

        public event EventHandler Saved;
        public event EventHandler Closed;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int height;
            if (int.TryParse(optHeight.Text, out height) == false)
            {
                MessageBox.Show("Invalid Height");
                return;
            }

            int width;
            if(int.TryParse(optWidth.Text, out width) == false)
            {
                MessageBox.Show("Invalid Width");
                return;
            }

            MapHeight = height;
            MapWidth = width;

            Saved?.Invoke(this, new EventArgs());
        }
    }
}
