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

namespace UnnamedStrategyGame.UI.GameTypeViews
{
    /// <summary>
    /// Interaction logic for ActionTypeView.xaml
    /// </summary>
    public partial class ActionTypeView : UserControl
    {
        private Game.ActionType _actionType { get; set; }
        public Game.ActionType ActionType
        {
            get { return _actionType; }
            set
            {
                _actionType = value;
                Update();
            }
        }

        public ActionTypeView()
        {
            InitializeComponent();
        }

        private void Update()
        {
            lblHeader.Content = Globals.GetResource(ActionType.Key);
            lblDesc.Content = Globals.GetResource($"{ActionType.Key}_desc");

            if(ActionType is Game.ActionTypes.ForUnits.AttackBase)
            {
                var attackType = (Game.ActionTypes.ForUnits.AttackBase)ActionType;

                lblAccuracy.Content = $"{attackType.BaseAccuracy / 100.0:0%;-0%}";
                lblArmorPenetration.Content = attackType.ArmorPenetration;
                lblActionsNeeded.Content = attackType.ActionsNeeded;
                lblCounterattack.Content = attackType.CanRetaliate;

                if (attackType.MinimumRange == attackType.MaximumRange)
                    lblRange.Content = $"{attackType.MinimumRange} {(attackType.MinimumRange == 1 ? "tile" : "tiles")}";
                else
                    lblRange.Content = $"{attackType.MinimumRange} to {attackType.MaximumRange} tiles";

                gridSupplyUsage.RowDefinitions.Clear();
                gridSupplyUsage.Children.Clear();

                {
                    var row = 0;

                    Resource.GenerateRow(gridSupplyUsage, ref row, "Supply", "Amount Used");

                    foreach (var kp in attackType.SuppliesNeeded)
                    {
                        var supplyType = kp.Key;
                        var suppliesUsed = kp.Value;

                        Resource.GenerateRow(gridSupplyUsage, ref row, Globals.GetResource(supplyType.Key), suppliesUsed);
                    }

                    if (attackType.SuppliesNeeded.Count == 0)
                    {
                        Resource.GenerateRow(gridSupplyUsage, ref row, "(none)");
                    }
                }


                VisToggle(Visibility.Visible);
            }
            else
            {
                VisToggle(Visibility.Collapsed);
            }


            var unitAction = ActionType as Game.ActionTypes.UnitAction;
            var terrainAction = ActionType as Game.ActionTypes.TerrainAction;
            var commanderAction = ActionType as Game.ActionTypes.CommanderAction;
            var gameAction = ActionType as Game.ActionTypes.GameAction;


            lblTriggers.Children.Clear();
            if(unitAction != null)
            {
                lblCategory.Content = "Unit Action";
                foreach (var trigger in Enum.GetValues(typeof(Game.ActionTypes.UnitAction.ActionTriggers)))
                {
                    if (unitAction.Triggers.HasFlag((Enum)trigger) && ((int)trigger) != 0)
                        lblTriggers.Children.Add(new Label() { Content = trigger });

                }
            }
            else if (terrainAction != null)
            {
                lblCategory.Content = "Terrain Action";
                foreach (var trigger in Enum.GetValues(typeof(Game.ActionTypes.TerrainAction.ActionTriggers)))
                {
                    if (terrainAction.Triggers.HasFlag((Enum)trigger) && ((int)trigger) != 0)
                        lblTriggers.Children.Add(new Label() { Content = trigger });

                }
            }
            else if (commanderAction != null)
            {
                lblCategory.Content = "Commander Action";
                foreach (var trigger in Enum.GetValues(typeof(Game.ActionTypes.CommanderAction.ActionTriggers)))
                {
                    if (commanderAction.Triggers.HasFlag((Enum)trigger) && ((int)trigger) != 0)
                        lblTriggers.Children.Add(new Label() { Content = trigger });

                }
            }
            else if (gameAction != null)
            {
                lblCategory.Content = "Game Action";
                foreach (var trigger in Enum.GetValues(typeof(Game.ActionTypes.GameAction.ActionTriggers)))
                {
                    if (gameAction.Triggers.HasFlag((Enum)trigger) && ((int)trigger) != 0)
                        lblTriggers.Children.Add(new Label() { Content = trigger });

                }
            }
        }

        private void VisToggle(Visibility vis)
        {
            lblAccuracy.Visibility = vis;
            lblArmorPenetration.Visibility = vis;
            lblActionsNeeded.Visibility = vis;
            lblCounterattack.Visibility = vis;
            gridSupplyUsage.Visibility = vis;
            lblRange.Visibility = vis;

            lblForAccuracy.Visibility = vis;
            lblForActionsNeeded.Visibility = vis;
            lblForArmorPenetration.Visibility = vis;
            lblForCounterattack.Visibility = vis;
            lblForSupplyUsage.Visibility = vis;
            lblRange.Visibility = vis;
        }
    }
}
