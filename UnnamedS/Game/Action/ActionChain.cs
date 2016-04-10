using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class ActionChain
    {
        private List<Link> Actions { get; } = new List<Link>();

        public int Length
        {
            get { return Actions.Count; }
        }

        public ActionChain() { }

        public ActionChain(List<Link> actions)
        {
            Contract.Requires<ArgumentNullException>(null != actions);

            Actions = actions;
        }

        public ActionChain(params Link[] actions)
        {
            Actions = actions.ToList();
        }

        public void AddAction(ActionType type, Location source, Location target)
        {
            Contract.Requires<ArgumentNullException>(null != type);
            Contract.Requires<ArgumentNullException>(null != source);
            Contract.Requires<ArgumentNullException>(null != target);

            Actions.Add(new Link(type, source, target));
        }

        public void AddAction(Link link)
        {
            Contract.Requires<ArgumentNullException>(null != link);

            Actions.Add(link);
        }

        public List<ActionInfo> GetActionsInfo(int commanderID, ActionType.ActionTriggers trigger)
        {
            var list = new List<ActionInfo>(Actions.Count);

            foreach(var link in Actions)
            {
                list.Add(new ActionInfo(commanderID, link.Action, link.Source, link.Target, trigger));
            }

            return list;
        }

        public List<ActionType> GetActions()
        {
            return Actions.Select(a => a.Action).ToList();
        }

        public class Link
        {
            public ActionType Action { get; }
            public Location Source { get; }
            public Location Target { get; }

            public Link(ActionType action, Location source, Location target)
            {
                Contract.Requires<ArgumentNullException>(null != action);
                Contract.Requires<ArgumentNullException>(null != source);
                Contract.Requires<ArgumentNullException>(null != target);

                Action = action;
                Source = source;
                Target = target;
            }
        }
    }
}
