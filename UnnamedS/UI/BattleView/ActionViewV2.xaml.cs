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
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for ActionViewV2.xaml
    /// </summary>
    public partial class ActionViewV2 : UserControl
    {
        public int CommanderID { get; private set; }

        public SourceContext SourceContext { get; private set; }

        public IReadOnlyList<ActionType> Actions { get; private set; }

        public IReadOnlyBattleGameState State { get; }

        public BattleViewV2 View { get; }

        public ActionViewV2(BattleViewV2 view, IReadOnlyBattleGameState state)
        {
            InitializeComponent();

            View = view; 
            State = state;            
        }

        public void DisplayActions(IReadOnlyList<ActionType> actions, SourceContext sourceContext, int commanderID)
        {
            CommanderID = commanderID;
            SourceContext = sourceContext;
            Actions = actions;

            actionListStackPanel.Children.Clear();

            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                if (action.TargetValueTypes.Length == 0 && action.CanPerformOn(State, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, sourceContext, new OtherContext())) == false)
                    continue;

                actionListStackPanel.Children.Add(CreateActionButton(i));
            }
        }

        private Button CreateActionButton(int actionIndex)
        {
            var action = Actions[actionIndex];

            var button = new Button() { Content = Globals.GetResource(action.Key), Tag = actionIndex, Margin = new Thickness(10), Padding = new Thickness(15), BorderBrush = null };
            button.Click += ActionSelected;

            return button;
        }

        private void ActionSelected(object sender, RoutedEventArgs e)
        {
            View.TriggerAction(Actions[(int)(sender as Button).Tag], CommanderID, SourceContext, new GenericContext());
        }
    }
}
