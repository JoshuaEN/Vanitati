using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TargetContext : Context
    {
        public sealed override bool CanBeSource { get; } = false;
        public sealed override bool CanBeTarget { get; } = true;
        public sealed override ActionType.Category ActionCategory
        {
            get { throw new NotSupportedException(); }
        }

        public abstract IReadOnlyList<object> Values { get; }
        public abstract IReadOnlyList<string> ValueTypes { get; }

        [Pure]
        public static bool IsValidGenericValueType(Type valueType)
        {
            foreach (var type in ActionType.GENERIC_ACTION_TYPE_VALUES.Values)
            {
                if (type.IsAssignableFrom(valueType))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
