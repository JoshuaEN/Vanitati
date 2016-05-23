using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UnnamedStrategyGame.Game.Action;
using System.Net;
using System.Net.Sockets;

namespace UnnamedStrategyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, UI.Settings.IUsesSettings
    {
        private Network.Server _server;
        public Network.Server Server
        {
            get { return _server; }
            set
            {
                _server = value;
                viewer?.AddSource("Server", _server);
            }
        }

        public bool NoClosingCancelPrompt { get; set; } = false;

        private void Server_Exception(object sender, Network.ExceptionEventArgs e)
        {

        }

        private void Server_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {

        }

        public UI.NetworkLogViewer viewer;

        public MainWindow()
        {
            InitializeComponent();

            Preloader.Preload();

            UI.Settings.Settings.Current.DisplayModeChanged += Current_DisplayDataChanged;
            UI.Settings.Settings.Current.DisplayStateChanged += Current_DisplayDataChanged;
            UI.Settings.Settings.Current.DisplaySizeChanged += Current_DisplayDataChanged;

            viewer = new UI.NetworkLogViewer();
            //viewer.Show();
            //if(false)
            //    Content = new UI.DamageTable();
            //else
            //    Content = new UI.BattleViewV2();
            //return;

        }

        private void Current_DisplayDataChanged(object sender, EventArgs e)
        {
            SettingsUpdated();
        }

        public void SetContent(FrameworkElement elm)
        {
            mainContent.Content = elm;   
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsUpdated();
            UI.Resource.MainWindow = this;
            //SetContent(new UI.BattleViewV2());
            SetContent(new UI.MainMenu());
            Visibility = Visibility.Visible;
        }

        public void SettingsUpdated()
        {
            if(UI.Settings.Settings.Current.DisplayMode == UI.Settings.Settings.WindowDisplayMode.BorderlessFullscreen)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                Height = SystemParameters.FullPrimaryScreenHeight;
                Width = SystemParameters.FullPrimaryScreenWidth;
            }
            else if(UI.Settings.Settings.Current.DisplayMode == UI.Settings.Settings.WindowDisplayMode.Fullscreen)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.CanMinimize;
            }
            else
            {
                var height = UI.Settings.Settings.Current.DisplaySize.Height;
                var width = UI.Settings.Settings.Current.DisplaySize.Width;
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = UI.Settings.Settings.Current.DisplayState;
                ResizeMode = ResizeMode.CanResize;
                Height = height;
                Width = width;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(UI.Settings.Settings.Current.DisplayMode == UI.Settings.Settings.WindowDisplayMode.Windowed && IsLoaded)
                UI.Settings.Settings.Current.DisplaySize = new Size(Width, Height);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(NoClosingCancelPrompt == true || MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            UI.Settings.Settings.SaveSettings();
        }

        private void MessageReset()
        {
            msgContent.RowDefinitions.Clear();
            msgContent.ColumnDefinitions.Clear();
            msgContent.Children.Clear();
            msgContainer.Visibility = Visibility.Visible;
        }

        public void ShowMessage(Exception ex)
        {
            MessageReset();

            var rootEx = ex;

            while (rootEx.InnerException != null)
                rootEx = rootEx.InnerException;

            msgHeader.Content = rootEx.Message;

            msgContent.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            msgContent.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });

            var row = 0;
            UI.Resource.GenerateRow(msgContent, ref row, "Type", rootEx.GetType().Name);


            msgContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            msgContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            msgContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });


            var stackLbl = new Label() { Content = "Stack Trace" };
            Grid.SetRow(stackLbl, row++);
            Grid.SetColumnSpan(stackLbl, 2);
            msgContent.Children.Add(stackLbl);
            var stackTxt = new TextBox() { Text = ex.StackTrace, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Visible, TextWrapping = TextWrapping.NoWrap, IsReadOnly = true };
            Grid.SetRow(stackTxt, row++);
            Grid.SetColumnSpan(stackTxt, 2);
            msgContent.Children.Add(stackTxt);
            stackTxt = new TextBox() { Text = rootEx.StackTrace, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Visible, TextWrapping = TextWrapping.NoWrap, IsReadOnly = true };
            Grid.SetRow(stackTxt, row++);
            Grid.SetColumnSpan(stackTxt, 2);
            msgContent.Children.Add(stackTxt);
        }

        public void ShowFatalMessage(Exception ex)
        {
            ShowMessage(ex);
            var stacky = new StackPanel();
            stacky.Children.Add(new Label() { Content = "Fatal Error", FontSize = 40 });
            stacky.Children.Add(new Label() { Content = msgHeader.Content });
            msgHeader.Content = stacky;
        }

        public void ShowMessage(object header, string content)
        {
            MessageReset();
            msgHeader.Content = header;
            msgContent.Children.Add(new Label() { Content = content});
        }

        public void ShowMessage(object header, UIElement content)
        {
            MessageReset();
            msgHeader.Content = header;
            msgContent.Children.Add(content);
        }

        private void msgCloseButton_Click(object sender, RoutedEventArgs e)
        {
            msgContainer.Visibility = Visibility.Collapsed;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (UI.Settings.Settings.Current.DisplayMode == UI.Settings.Settings.WindowDisplayMode.Windowed && IsLoaded)
                UI.Settings.Settings.Current.DisplayState = WindowState;
        }
    }
}
