using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    /// <summary>
    /// Contextual information related to calling an action.
    /// </summary>
    public class ActionContext
    {
        public int? TriggeredByCommanderID { get; }
        public ActionType.Category ActionCategory { get { return Source.ActionCategory; } }
        public ActionType.TargetCategory ActionTargetCategory { get { return Target.ActionTargetCategory; } }
        public Context Source { get; }
        public Context Target { get; }

        [Newtonsoft.Json.JsonConverter(typeof(Serializers.JsonConverters.ActionTriggerEnumConverter))]
        public GenericActionTrigger ActionTrigger { get; }

        [JsonIgnore]
        public Enum Trigger { get { return ActionTrigger.Trigger; } }
        
        [JsonIgnore]
        public bool TriggeredManuallyByUser { get { return ActionTrigger.TriggeredManuallyByUser; } }

        public ActionContext(int? triggeredByCommanderID, TriggerAutoDetermineMode mode, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            Contract.Requires<ArgumentException>(mode == TriggerAutoDetermineMode.ManuallyByUser, "Invalid Mode");

            if (source is UnitContext)
                ActionTrigger = new GenericActionTrigger(ActionTypes.UnitAction.ActionTriggers.ManuallyByUser);
            else if (source is TerrainContext)
                ActionTrigger = new GenericActionTrigger(ActionTypes.TerrainAction.ActionTriggers.ManuallyByUser);
            else if (source is CommanderContext)
                ActionTrigger = new GenericActionTrigger(ActionTypes.CommanderAction.ActionTriggers.ManuallyByUser);
            else
                throw new ArgumentException($"Context of {source.GetType()} does not support trigger auto-determination");
        }

        [JsonConstructor]
        public ActionContext(int? triggeredByCommanderID, GenericActionTrigger actionTrigger, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            Contract.Requires<ArgumentNullException>(null != actionTrigger);
            ActionTrigger = actionTrigger;
        }

        public ActionContext(int? triggeredByCommanderID, ActionTypes.UnitAction.ActionTriggers trigger, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            ActionTrigger = new GenericActionTrigger(trigger);
        }

        public ActionContext(int? triggeredByCommanderID, ActionTypes.TerrainAction.ActionTriggers trigger, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            ActionTrigger = new GenericActionTrigger(trigger);
        }

        public ActionContext(int? triggeredByCommanderID, ActionTypes.CommanderAction.ActionTriggers trigger, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            ActionTrigger = new GenericActionTrigger(trigger);
        }

        public ActionContext(int? triggeredByCommanderID, ActionTypes.GameAction.ActionTriggers trigger, Context source, Context target) : this(triggeredByCommanderID, source, target)
        {
            ActionTrigger = new GenericActionTrigger(trigger);
        }

        private ActionContext(int? triggeredByCommanderID, Context source, Context target)
        {
            Contract.Requires<ArgumentNullException>(null != source);
            Contract.Requires<ArgumentNullException>(null != target);
            Contract.Requires<ArgumentException>(source.CanBeSource);
            Contract.Requires<ArgumentException>(target.CanBeTarget);


            TriggeredByCommanderID = triggeredByCommanderID;
            Source = source;
            Target = target;
        }

        public void ConvertToSpecificContext<TSource, TTarget>(TargetContextBase.Load load, out TSource source, out TTarget target) where TSource:Context where TTarget:Context
        {
            source = (Source as TSource);
            target = (Target as TTarget);

            if (load.HasFlag(TargetContextBase.Load.Source))
            {
                if (source == null)
                    throw new ArgumentException($"Action context's source type is {Source.GetType()}, not {typeof(TSource)}");

            }

            if (load.HasFlag(TargetContextBase.Load.Target))
            {
                if (target == null)
                    throw new ArgumentException($"Action context's source type is {Target.GetType()}, not {typeof(TTarget)}");

                if (target is NullContext)
                    throw new ArgumentException("Cannot convert NullContext");
            }
        }

        public class GenericActionTrigger
        {
            public Enum Trigger { get; }
            public string TriggerType { get { return Trigger.GetType().FullName; } }
            [JsonIgnore]
            public bool TriggeredManuallyByUser { get; }

            public GenericActionTrigger(ActionTypes.UnitAction.ActionTriggers trigger) : this((Enum)trigger)
            {
                TriggeredManuallyByUser = trigger == ActionTypes.UnitAction.ActionTriggers.ManuallyByUser;
            }

            public GenericActionTrigger(ActionTypes.TerrainAction.ActionTriggers trigger) : this((Enum)trigger)
            {
                TriggeredManuallyByUser = trigger == ActionTypes.TerrainAction.ActionTriggers.ManuallyByUser;
            }

            public GenericActionTrigger(ActionTypes.CommanderAction.ActionTriggers trigger) : this((Enum)trigger)
            {
                TriggeredManuallyByUser = trigger == ActionTypes.CommanderAction.ActionTriggers.ManuallyByUser;
            }

            public GenericActionTrigger(ActionTypes.GameAction.ActionTriggers trigger) : this((Enum)trigger)
            {
                TriggeredManuallyByUser = false;
            }

            private GenericActionTrigger(Enum trigger)
            {
                Contract.Requires<ArgumentNullException>(null != trigger);

                Trigger = trigger;
            }
        }
        public enum TriggerAutoDetermineMode { Unset, ManuallyByUser }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Source);
            Contract.Invariant(null != Target);
            Contract.Invariant(null != ActionTrigger);
            Contract.Invariant(Source is UnitContext == false || Trigger is ActionTypes.UnitAction.ActionTriggers);
            Contract.Invariant(Source is TerrainContext == false || Trigger is ActionTypes.TerrainAction.ActionTriggers);
            Contract.Invariant(Source is CommanderContext == false || Trigger is ActionTypes.CommanderAction.ActionTriggers);
            Contract.Invariant(Source is GameContext == false || Trigger is ActionTypes.GameAction.ActionTriggers);
            Contract.Invariant(Source is UnitContext || Source is TerrainContext || Source is CommanderContext || Source is GameContext);
        }
    }
}
