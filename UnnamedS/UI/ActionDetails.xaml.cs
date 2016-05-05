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
    /// Interaction logic for ActionDetails.xaml
    /// </summary>
    public partial class ActionDetails : UserControl
    {
        private IEnumerable<Game.ActionType.Modifier> _modifiers;
        public IEnumerable<Game.ActionType.Modifier> Modifiers
        {
            get { return _modifiers; }
            set
            {
                _modifiers = value;
                Update();
            }
        }

        public ActionDetails()
        {
            InitializeComponent();
        }

        public void Update()
        {
            actionModifierList.Children.Clear();

            foreach(var modifer in Modifiers)
            {
                ProcessModifier(actionModifierList, modifer);
            }
        }

        private void ProcessModifier(Panel parentPanel, Game.ActionType.Modifier modifier)
        {
            var panel = new StackPanel() { Margin = new Thickness(10, 0, 5, 0) };

            var title = new TextBlock();

            var mod_key = "action_modifier_" + modifier.Key;

            if (modifier is Game.ActionType.ModifierList)
                title.Text = string.Format(Globals.GetResource(mod_key), (modifier as Game.ActionType.ModifierList).Values.ToArray());
            else
                title.Text = string.Format(Globals.GetResource(mod_key), modifier.Value);

            panel.Children.Add(title);

            foreach (var childModifier in modifier.Items)
            {
                ProcessModifier(panel, childModifier);
            }

            parentPanel.Children.Add(panel);
        }
    }
}
