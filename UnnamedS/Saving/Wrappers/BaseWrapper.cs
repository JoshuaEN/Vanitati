using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnnamedStrategyGame.Saving.Wrappers
{
    public abstract class BaseWrapper
    {
        public Game.BattleGameState.Fields Fields { get; }

        [JsonProperty]
        protected string TypeKey { get { return GetType().Name; } }

        [JsonIgnore]
        public abstract SaveType Type { get; }
        [JsonIgnore]
        public abstract string Extension { get; }

        protected BaseWrapper(Game.BattleGameState.Fields fields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);

            Fields = fields;
        }

        protected BaseWrapper() {}

        public enum SaveType { Map, SaveGame }


        public static Dictionary<string, BaseWrapper> TYPES { get; private set; }

        public static void LoadBaseWrapperTypes()
        {
            if (TYPES != null)
                throw new NotSupportedException();

            var listing = new Dictionary<string, BaseWrapper>();

            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && typeof(BaseWrapper).IsAssignableFrom(t) && t.IsAbstract == false && t.Namespace == "UnnamedStrategyGame.Saving.Wrappers"
                        select t;

            foreach (var t in types)
            {

                var prop = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if (prop == null)
                {
                    throw new ArgumentException(String.Format("Game Type {0} must declare a public static property Instance", t));
                }

                var res = (BaseWrapper)prop.GetValue(null);

                listing.Add(res.TypeKey, res);
            }

            TYPES = listing;
        }
    }
}
