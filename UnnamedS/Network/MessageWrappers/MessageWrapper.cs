using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class MessageWrapper
    {
        public string Type
        {
            get { return GetType().FullName; }
        }

        public static readonly IReadOnlyDictionary<string, Type> WRAPPER_LISTING;

        static MessageWrapper()
        {
            var listing = new Dictionary<string, Type>();

            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && typeof(MessageWrapper).IsAssignableFrom(t) && t.IsAbstract == false && t.Namespace == "UnnamedStrategyGame.Network.MessageWrappers"
                        select t;

            foreach (var t in types)
            {
                listing.Add(t.FullName, t);
            }

            WRAPPER_LISTING = listing;
        }
    }
}
