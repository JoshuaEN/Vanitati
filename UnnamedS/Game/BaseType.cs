using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class BaseType
    {
        public string Key { get; }

        protected BaseType(string key)
        {
            Contract.Requires<ArgumentNullException>(null != key);

            Key = key;
        }

        /// <summary>
        /// Dynamically builds a listing (dictionary) of a given Type within the given nameSpace.
        /// This removes possible errors in omitting Types if this process needed to be performed manually.
        /// </summary>
        /// <typeparam name="T">The Base Type of the Types to generate a list of.</typeparam>
        /// <param name="nameSpace">The namespace to search.</param>
        /// <returns>Dictionary listing of the Types.</returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static Dictionary<string, T> BuildTypeListing<T>(string nameSpace) where T : BaseType
        {
            var listing = new Dictionary<string, T>();

            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && typeof(T).IsAssignableFrom(t) && t.IsAbstract == false && t.Namespace == nameSpace
                        select t;

            foreach (var t in types)
            {
                //if (t.IsSealed == false)
                //{
                //    throw new Exceptions.InvalidDefinitionException(String.Format("Game Type {0} must be declared as sealed", t));
                //}

                var prop = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if (prop == null)
                {
                    throw new Exceptions.InvalidDefinitionException(String.Format("Game Type {0} must declare a public static property Instance", t));
                }

                var res = (T)prop.GetValue(null);

                listing.Add(res.Key, res);
            }
            return listing;
        }

        public override string ToString()
        {
            return Key;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Key);
        }
    }
}
