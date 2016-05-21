using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class Context
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual bool CanBeSource { get { return true; } }
        [Newtonsoft.Json.JsonIgnore]
        public virtual bool CanBeTarget { get { return true; } }
        [Newtonsoft.Json.JsonIgnore]
        public abstract ActionType.Category ActionCategory { get; }

        public string Type
        {
            get { return GetType().Name; }
        }

        public static readonly IReadOnlyDictionary<string, Type> CONTEXT_LISTING;

        static Context()
        {
            var listing = new Dictionary<string, Type>();

            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && typeof(Context).IsAssignableFrom(t) && t.IsAbstract == false && t.Namespace == "UnnamedStrategyGame.Game.Action"
                        select t;

            foreach (var t in types)
            {
                listing.Add(t.Name, t);
            }

            CONTEXT_LISTING = listing;
        }
    }
}
