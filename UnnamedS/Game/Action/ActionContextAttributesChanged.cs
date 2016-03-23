using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class ActionContextAttributesChanged : ActionContext
    {
        public IReadOnlyList<IAttribute> ChangedAttributes { get; }
        public ActionContextAttributesChanged(List<IAttribute> changedAttributes) : base(ActionType.ActionTriggers.AttributeChange)
        {
            ChangedAttributes = changedAttributes;
        }
    }
}
