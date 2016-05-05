using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    /// <summary>
    /// Represents a sequence of actions
    /// </summary>
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

            foreach(var link in actions)
            {
                AddAction(link);
            }
        }

        public ActionChain(params Link[] actions)
        {
            Contract.Requires<ArgumentNullException>(null != actions);

            foreach(var link in actions)
            {
                AddAction(link);
            }
        }

        public void AddAction(ActionType type, Context source, Context target)
        {
            Contract.Requires<ArgumentNullException>(null != type);
            Contract.Requires<ArgumentNullException>(null != source);
            Contract.Requires<ArgumentNullException>(null != target);
            Contract.Requires<ArgumentException>(type.CanUserTrigger == true);

            Actions.Add(new Link(type, source, target));
        }

        public void AddAction(Link link)
        {
            Contract.Requires<ArgumentNullException>(null != link);
            Contract.Ensures(Actions.Contains(link));

            Actions.Add(link);
        }

        public List<ActionInfo> GetActionsInfo(int commanderID)
        {
            Contract.Ensures(Contract.Result<List<ActionInfo>>() != null);
            Contract.Ensures(Contract.Result<List<ActionInfo>>().Count == Actions.Count);

            return Actions.Select(link => new ActionInfo(link.Action, new ActionContext(commanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, link.Source, link.Target))).ToList();
        }

        public List<ActionType> GetActions()
        {
            Contract.Ensures(Contract.Result<List<ActionType>>() != null);
            Contract.Ensures(Contract.Result<List<ActionType>>().Count == Actions.Count);

            return Actions.Select(a => a.Action).ToList();
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Actions);
        }

        public class Link
        {
            public ActionType Action { get; }
            public Context Source { get; }
            public Context Target { get; }

            public Link(ActionType action, Context source, Context target)
            {
                Contract.Requires<ArgumentNullException>(null != action);
                Contract.Requires<ArgumentNullException>(null != source);
                Contract.Requires<ArgumentNullException>(null != target);

                Action = action;
                Source = source;
                Target = target;
            }

            [ContractInvariantMethod]
            private void Invariants()
            {
                Contract.Invariant(null != Action);
                Contract.Invariant(null != Source);
                Contract.Invariant(null != Target);
            }
        }
    }
}
