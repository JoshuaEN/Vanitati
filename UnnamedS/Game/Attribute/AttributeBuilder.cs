using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game
{
    public sealed class AttributeBuilder
    {
        public IReadOnlyDictionary<string, IAttributeDefinition> Definitions { get; }
        
        public AttributeBuilder(Dictionary<string, IAttributeDefinition> definitions)
        {
            Contract.Requires<ArgumentNullException>(definitions != null);
            Definitions = definitions;
        }

        public AttributeBuilder(params IAttributeDefinition[] definitions)
        {
            Contract.Requires<ArgumentNullException>(definitions != null);
            Contract.Requires<ArgumentException>(definitions.Length > 0, "Must provide at least one Attribute Definition");
            Contract.Ensures(Definitions != null);
            Contract.Ensures(Definitions.Count == definitions.Length);

            var defs = new Dictionary<string, IAttributeDefinition>(definitions.Length);
            Contract.Assume(defs != null);

            foreach (var def in definitions)
            {
                defs.Add(def.Key, def);
            }

            Definitions = defs;
        }

        public List<IAttribute> BuildFullAttributeList(Dictionary<string, object> attributes, bool readOnly = false)
        {
            Contract.Requires<ArgumentNullException>(attributes != null);

            var returnAttributes = new List<IAttribute>(Definitions.Count);

            foreach(var def in Definitions)
            {
                object value;
                if (attributes.TryGetValue(def.Key, out value) == false)
                {
                    if (def.Value.HasDefaultValue == false)
                    {
                        throw new Exceptions.MissingAttributeException("Missing Required Attribute: " + def.Key);
                    }
                    returnAttributes.Add(def.Value.GetDefaultAttribute(readOnly));
                }
                else
                {
                    returnAttributes.Add(def.Value.GetAttribute(value, readOnly));
                }

                attributes.Remove(def.Key);
            }

            if(attributes.Count > 0)
            {
                Contract.Assume(attributes.Count() > 0);
                throw new Exceptions.UnknownAttributeException("Unknown Attribute: " + attributes.First().Key);
            }
            
            return returnAttributes;
        }

        public List<IAttribute> BuildFullAttributeList(IAttribute[] attributes, bool readOnlyForDefaults = false)
        {
            Contract.Requires<ArgumentNullException>(attributes != null);

            var returnAttributes = new List<IAttribute>(Definitions.Count);

            var attributeDict = new Dictionary<string, IAttribute>(Definitions.Count);

            var missingAttributes = Definitions.ToDictionary(kp => kp.Key, kp => kp.Value);

            foreach(var attr in attributes)
            {
                IAttributeDefinition def;
                if (Definitions.TryGetValue(attr.Key, out def))
                {
                    if (attr.Definition == def)
                    {
                        returnAttributes.Add(attr);
                        missingAttributes.Remove(def.Key);
                        continue;
                    }
                    else
                    {
                        throw new Exceptions.AttributeDefinitionMismatchException(String.Format("Attribute definition of {0} does not match expected definition of {1}", attr.Definition, def));
                    }
                }
                else
                {
                    throw new Exceptions.UnknownAttributeException("Unknown Attribute: " + attr.Key);
                }
                
            }

            foreach(var def in missingAttributes)
            {
                if (def.Value.HasDefaultValue == false)
                {
                    throw new Exceptions.MissingAttributeException("Missing Required Attribute: " + def.Key);
                }
                returnAttributes.Add(def.Value.GetDefaultAttribute(readOnlyForDefaults));
            }

            return returnAttributes;
        }

        public IAttribute BuildDefaultAttribute(string key, bool readOnly = false)
        {
            return GetDefinition(key).GetDefaultAttribute(readOnly);
        }

        public IAttribute BuildAttribute(string key, object value, bool readOnly = false)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return GetDefinition(key).GetAttribute(value, readOnly);
        }

        public List<IAttribute> BuildAttributes(Dictionary<string, object> attributes, bool readOnly = false)
        {
            Contract.Requires<ArgumentException>(attributes.Count > 0, "At least one attribute must be specified.");

            var list = new List<IAttribute>(attributes.Count);

            foreach(var keyPair in attributes)
            {
                list.Add(BuildAttribute(keyPair.Key, keyPair.Value, readOnly));
            }

            return list;
        }

        private IAttributeDefinition GetDefinition(string key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            IAttributeDefinition def;

            if (Definitions.TryGetValue(key, out def))
            {
                return def;
            }
            else
            {
                throw new Exceptions.UnknownAttributeException("Unknown Attribute: " + key);
            }
        }
    }
}
