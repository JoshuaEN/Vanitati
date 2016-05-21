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
    /// Interaction logic for UserToCommanderAssignment.xaml
    /// </summary>
    public partial class UserToCommanderAssignment : UserControl
    {
        private BattleViewV2 _view;
        public BattleViewV2 View
        {
            get { return _view; }
            set
            {
                _view = value;
                Update();
            }
        }

        public UserToCommanderAssignment()
        {
            InitializeComponent();
        }

        public event EventHandler<CommandUserLinkEventArgs> TakeCommand;
        public event EventHandler<CommandUserLinkEventArgs> ReleaseCommand;

        public void Update()
        {
            mainGrid.RowDefinitions.Clear();
            mainGrid.Children.Clear();

            var count = View?.Logic?.CommanderAssignments?.Count;

            if (null == count || count <= 0)
                return;


            var row = 0;

            Resource.GenerateRow(mainGrid, ref row, "User", "Commander", "");

            foreach(var kp in View.Logic.CommanderAssignments)
            {
                var commanderID = kp.Key;
                var userID = kp.Value;

                Commander commander = View.Logic.State.GetCommander(commanderID);
                User user = null;

                if (null != userID)
                    user = View.Logic.Users[userID.Value];

                string text;
                Brush background;
                Resource.GetCommanderIDNameAndColor(View.Logic, commanderID, out text, out background);

                var button = new Button();

                if (null == userID)
                {
                    button.Content = "Take Command";
                    button.Click += TakeCommandButton_Click;
                }
                else if(user == View.OurUser)
                {
                    button.Content = "Relinquish Command";
                    button.Click += ReleaseCommandButton_Click;
                }
                else if(View.OurUser.IsHost)
                {
                    button.Content = "Override Command";
                    button.Click += TakeCommandButton_Click;
                }
                else
                {
                    button.Content = "Take Command";
                    button.IsEnabled = false;
                }

                button.Tag = commanderID;


                Resource.GenerateRow(mainGrid, ref row, text, new Label() { Background = background }, button);
            }
        }

        private void ReleaseCommandButton_Click(object sender, RoutedEventArgs e)
        {
            ReleaseCommand?.Invoke(this, new CommandUserLinkEventArgs((int)(sender as Button).Tag, null));
        }

        private void TakeCommandButton_Click(object sender, RoutedEventArgs e)
        {
            TakeCommand?.Invoke(this, new CommandUserLinkEventArgs((int)(sender as Button).Tag, View.OurUser.UserID));
        }

        public class CommandUserLinkEventArgs : EventArgs
        {
            public int CommanderID { get; }
            public int? UserID { get; }

            public CommandUserLinkEventArgs(int commanderID, int? userID)
            {
                CommanderID = commanderID;
                UserID = userID;
            }
        }
    }
}
