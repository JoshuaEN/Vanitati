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
    /// Interaction logic for CommanderView.xaml
    /// </summary>
    public partial class CommandersView : UserControl
    {
        
        public BattleViewV2 View { get; set; }

        public CommandersView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            commanderGrid.ColumnDefinitions.Clear();
            commanderGrid.RowDefinitions.Clear();
            commanderGrid.Children.Clear();

            commanderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            commanderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });


            int vpMax = 0;
            int vp2nd = 0;
            int vpPointGoal = 0;
            if (View.State.VictoryPointGapEnabled)
            {
                commanderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });

                foreach (var kp in View.State.Commanders)
                {
                    var vp = kp.Value.VictoryPoints;
                    if (vp > vpMax)
                    {
                        vp2nd = vpMax;
                        vpMax = vp;
                    }
                    else if (vp > vp2nd)
                    {
                        vp2nd = vp;
                    }
                }

                vpPointGoal = vp2nd + View.State.VictoryPointGap;
            }

            if (View.State.VictoryPointLimitEnabled)
            {
                if (View.State.VictoryPointGapEnabled)
                {
                    if (View.State.VictoryPointLimit < vpPointGoal)
                        vpPointGoal = View.State.VictoryPointLimit;

                }
                else
                {
                    vpPointGoal = View.State.VictoryPointLimit;
                    commanderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
                }
            }

            commanderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });

            var row = 0;

            foreach(var kp in View.State.Commanders)
            {
                var column = 0;
                var commander = kp.Value;

                commanderGrid.RowDefinitions.Add(new RowDefinition());

                string text; Brush background;

                Resource.GetCommanderIDNameAndColor(View.Logic, commander.CommanderID, out text, out background);

                var nameGrid = new StackPanel() { Orientation = Orientation.Vertical };
                var nameLabel = new Label() { Content = nameGrid };
                var name = new TextBlock() { Text = text };
                var color = new Label() { Background = background };
                var credits = new Label() { Content = $"{commander.Credits:C0}" };

                nameGrid.Children.Add(name);

                var subtexts = new List<string>();

                if (View.Logic.IsUserCommanding(View.OurUser.UserID, commander.CommanderID))
                {
                    subtexts.Add("You");
                }

                if(commander.CommanderID == View.State.CurrentCommander?.CommanderID)
                {
                    subtexts.Add("Current Turn");
                }

                nameGrid.Children.Add(new TextBlock() { Text = string.Join(", ", subtexts), FontSize = 10 });

                Grid.SetRow(nameLabel, row);
                Grid.SetRow(credits, row);

                Grid.SetColumn(nameLabel, column++);
                Grid.SetColumn(credits, column++);

                commanderGrid.Children.Add(nameLabel);
                commanderGrid.Children.Add(credits);


                if(View.State.VictoryPointGapEnabled || View.State.VictoryPointLimitEnabled)
                {
                    var vpLabel = new Label() { Content = $"{vpPointGoal - commander.VictoryPoints:N0} VP", ToolTip = "Victory Points remaining to Victory" };
                    Grid.SetRow(vpLabel, row);
                    Grid.SetColumn(vpLabel, column++);

                    commanderGrid.Children.Add(vpLabel);
                }

                Grid.SetRow(color, row);
                Grid.SetColumn(color, column++);
                commanderGrid.Children.Add(color);

                row++;

            }
        }
    }
}
