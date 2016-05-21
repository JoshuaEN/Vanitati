using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    /// Interaction logic for ActionView.xaml
    /// </summary>
    public partial class ActionView : UserControl
    {
        public int CommanderID { get; private set; }

        public SourceContext SourceContext { get; private set; }

        public ActionType Action { get; private set; }

        public IReadOnlyBattleGameState State { get; private set; }

        private ActionHandler Handler { get; set; }

        public ActionView(IReadOnlyBattleGameState state, ActionType action, SourceContext sourceContext, int commanderID)
        {
            InitializeComponent();
            SetAction(state, action, sourceContext, commanderID);
        }

        public event EventHandler<ActionTriggeredEventArgs> ActionTriggered;

        protected void OnActionTriggered(TargetContext targetContext)
        {
            ActionTriggered?.Invoke(this, new ActionTriggeredEventArgs(Action, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, SourceContext, targetContext)));
        }

        private void SetAction(IReadOnlyBattleGameState state, ActionType action, SourceContext sourceContext, int commanderID)
        {
            Contract.Requires<ArgumentNullException>(null != action);
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != sourceContext);

            SourceContext = sourceContext;
            State = state;
            Action = action;
            CommanderID = commanderID;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        { 
            switch(Action.ActionTargetCategory)
            {
                case ActionType.TargetCategory.Tile:
                    Handler = new TileActionHandler(this);
                    break;
                case ActionType.TargetCategory.Other:
                    Handler = new OtherActionHandler(this);
                    break;
                case ActionType.TargetCategory.Generic:
                    Handler = new GenericActionHandler(this);
                    break;
                default:
                    throw new NotSupportedException($"Actions of targeting category {Action.ActionTargetCategory} are not supported by the ActionView");
            }

            Handler.Update();
        }

        public class ActionTriggeredEventArgs : EventArgs
        {
            public ActionContext ActionContext { get; }
            public ActionType ActionType { get; }

            public ActionTriggeredEventArgs(ActionType actionType, ActionContext actionContext)
            {
                Contract.Requires<ArgumentNullException>(null != actionType);
                Contract.Requires<ArgumentNullException>(null != actionContext);

                ActionType = actionType;
                ActionContext = actionContext;
            }
        }

        private abstract class ActionHandler
        {
            public ActionView View { get; }

            public int CommanderID { get { return View.CommanderID; } }

            public SourceContext SourceContext { get { return View.SourceContext; } }

            public ActionType Action { get { return View.Action; } }

            public IReadOnlyBattleGameState State { get { return View.State; } }

            public ActionHandler(ActionView view)
            {
                View = view;
                View.triggerButton.Click += TriggerButton_Click;
            }

            protected virtual void TriggerButton_Click(object sender, RoutedEventArgs e) { }

            public virtual void Update()
            {
                View.triggerButton.Content = new TextBlock() { Text = Globals.GetResource(View.Action.Key) };
                View.triggerContextMenu.Items.Clear();
            }
        }

        private class TileActionHandler : ActionHandler
        {
            public TargetContext TargetContext { get; }

            public TileActionHandler(ActionView view) : base(view)
            {
                if (SourceContext is TileContext == false)
                    throw new NotSupportedException();

                TargetContext = new TargetContext((SourceContext as TileContext).Location);
            }

            public override void Update()
            {
                if(Action.CanPerformOn(State, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, SourceContext, TargetContext)) == false)
                {
                    View.Visibility = Visibility.Collapsed;
                    return;
                }
                base.Update();
            }

            protected override void TriggerButton_Click(object sender, RoutedEventArgs e)
            {
                View.OnActionTriggered(TargetContext);
            }
        }

        private class GenericActionHandler : ActionHandler
        {
            public GenericActionHandler(ActionView view) : base(view) { }

            public override void Update()
            {
                base.Update();
                foreach (var opt in Action.AvailableOptions(State, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, SourceContext, new NullContext())))
                {
                    if (Action.CanPerformOn(State, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, SourceContext, new TargetContext(opt))) == false)
                    {
                        continue;
                    }

                    var item = new MenuItem() { Header = new TextBlock() { Text = Globals.GetResource(opt?.ToString()) }, Tag = opt };
                    item.PreviewMouseDown += Item_PreviewMouseDown;
                    View.triggerContextMenu.Items.Add(item);
                }

                if (View.triggerContextMenu.Items.Count < 1)
                {
                    View.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            private void Item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
                var opt = (sender as MenuItem).Tag;
                View.OnActionTriggered(new TargetContext(opt));
            }

            protected override void TriggerButton_Click(object sender, RoutedEventArgs e)
            {
                e.Handled = true;
                View.triggerContextMenu.IsOpen = true;
            }
        }

        private class OtherActionHandler : ActionHandler
        {
            public OtherActionHandler(ActionView view) : base(view) { }

            public override void Update()
            {
                if (Action.CanPerformOn(State, new ActionContext(CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, SourceContext, new OtherContext())) == false)
                {
                    View.Visibility = Visibility.Collapsed;
                    return;
                }

                base.Update();
            }

            protected override void TriggerButton_Click(object sender, RoutedEventArgs e)
            {
                View.OnActionTriggered(new OtherContext());
            }
        }
    }
}
