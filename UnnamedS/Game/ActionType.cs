using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForActionType))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class ActionType : BaseType
    {
        public abstract Category ActionCategory { get; }
        public abstract TargetCategory ActionTargetCategory { get; }

        public abstract bool CanUserTrigger { get; }
        public virtual bool CanRetaliate { get; } = false;

        protected ActionType(string key) : base("action_" + key) {  }

        /// <summary>
        /// Determines if the given Source Tile can perform this action on the Target Tile.
        /// </summary>
        /// <param name="state">Game State</param>
        /// <param name="context">Context under which the Action is being performed</param>
        /// <param name="sourceTile">Source of the Action</param>
        /// <param name="targetTile">Target of the Action</param>
        /// <returns>True if the Action can be performed, false otherwise.</returns>
        [Pure]
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context);

        /// <summary>
        /// Performs the given action and returns the resulting state changes.
        /// </summary>
        /// <param name="state">Game State</param>
        /// <param name="context">Context under which the Action is being performed</param>
        /// <param name="sourceTile">Source of the Action</param>
        /// <param name="targetTile">Target of the Action</param>
        /// <returns>List of State Changes as a result of the Action</returns>
        [Pure]
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context);

        /// <summary>
        /// Determines all possible locations this Action can be performed given the Source Tile
        /// Takes into account possible chaining of several Actions together
        /// </summary>
        /// <param name="state">Game State</param>
        /// <param name="context">Context under which the Action is being performed</param>
        /// <returns></returns>
        [Pure]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public virtual IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, ActionChain>>() != null);

            if (ActionTargetCategory == TargetCategory.Tile)
                throw new NotImplementedException();
            else
                throw new NotSupportedException();
        }

        [Pure]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public virtual IReadOnlyList<object> AvailableOptions(IReadOnlyBattleGameState state, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Ensures(Contract.Result<IReadOnlyList<object>>() != null);

            if (ActionTargetCategory == TargetCategory.Generic)
                throw new NotImplementedException();
            else
                throw new NotSupportedException();
        }

        [Pure]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context);

        public static IReadOnlyDictionary<string, ActionType> TYPES { get; private set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void InitActionType()
        {
            if (TYPES != null)
                return;

            TYPES = BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForCommanders").
                Concat(BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForTerrain")).
                Concat(BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForUnits")).
                Concat(BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes.ForGame")).ToDictionary(kp => kp.Key, kp => kp.Value);

            InitGenericActionTypeValues();
        }

        public static IReadOnlyDictionary<string, Type> GENERIC_ACTION_TYPE_VALUES { get; private set; }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static void InitGenericActionTypeValues()
        {
            var dic = new Dictionary<string, Type>();

            foreach(var actionType in TYPES.Values)
            {
                var type = actionType.GetType();

                while (type != null && type != typeof(ActionType))
                {
                    if (type.IsGenericType == true)
                    {
                        var typeArg = type.GetGenericArguments()[0];

                        dic[typeArg.FullName] = typeArg;

                        break;
                    }
                    type = type.BaseType;
                }
            }

            GENERIC_ACTION_TYPE_VALUES = dic;
        }

        public enum Category { Unit, Terrain, Commander, Game }
        public enum TargetCategory { Tile, Commander, Generic, Other }

        [ContractInvariantMethod]
        private void Invariants()
        {
#if ACTION_TYPE_TEST_MODE
            Contract.Invariant(ActionCategory != Category.Unit || this is ActionTypes.UnitAction);
            Contract.Invariant(ActionCategory != Category.Terrain || this is ActionTypes.TerrainAction);
            Contract.Invariant(ActionCategory != Category.Commander || this is ActionTypes.CommanderAction);
#endif
        }

        public class Modifier
        {
            public string Key { get; }
            public object Value { get; }

            public IReadOnlyList<Modifier> Items { get; }

            public Modifier(string key, object value, params Modifier[] items)
            {
                Key = key;
                Value = value;
                Items = items.ToList();
            }
        }

        public class ModifierForumla : Modifier
        {
            public static readonly Modifier OPERATOR_PLUS = new Modifier("operator_plus", "+");
            public static readonly Modifier OPERATOR_MINUS = new Modifier("operator_minus", "-");
            public static readonly Modifier OPERATOR_MULTIPLY = new Modifier("operator_multiply", "*");
            public static readonly Modifier OPERATOR_DIVIDE = new Modifier("operator_divide", "/");
            public static readonly Modifier OPERATOR_LEFT_PARENTHESE = new Modifier("operator_left_parenthese", "(");
            public static readonly Modifier OPERATOR_RIGHT_PARENTHESE = new Modifier("operator_right_parenthese", ")");


            public ModifierForumla(string key, object value, params Modifier[] forumla) : base(key, value, forumla ?? new Modifier[0])
            {
            }
        }

        public class ModifierList : Modifier
        {
            public IEnumerable<object> Values { get; }
            public ModifierList(string key, params object[] values) : base(key, Array.AsReadOnly(values))
            {
                Values = Array.AsReadOnly(values);
            }
        }
    }

    [ContractClassFor(typeof(ActionType))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForActionType : ActionType
    {
        [Pure]
        public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);

            return false;
        }

        [Pure]
        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<NotSupportedException>(CanPerformOn(state, context));
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            return null;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<NotSupportedException>(CanPerformOn(state, context));
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            return null;
        }

        private ContractClassForActionType() : base(null) { }
    }
}
