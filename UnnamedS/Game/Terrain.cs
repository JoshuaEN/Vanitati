using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public class Terrain : IAttributeContainer
    {
        public IReadOnlyList<IAttribute> Attributes
        {
            get
            {
                return attributeContainer.Attributes;
            }
        }

        private IAttributeContainer attributeContainer;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { attributeContainer.PropertyChanged += value; }
            remove { attributeContainer.PropertyChanged -= value; }
        }

        public event EventHandler<AttributeChangedEventArgs> AttributeChanged
        {
            add { attributeContainer.AttributeChanged += value; }
            remove { attributeContainer.AttributeChanged -= value; }
        }

        public IAttribute GetAttribute(string key)
        {
            return attributeContainer.GetAttribute(key);
        }

        public void SetAttribute(IAttribute value)
        {
            attributeContainer.SetAttribute(value);
        }

        public void SetAttributeReadOnly(string key)
        {
            attributeContainer.SetAttributeReadOnly(key);
        }

        public void SetAttributes(IReadOnlyList<IAttribute> values)
        {
            attributeContainer.SetAttributes(values);
        }

        public Terrain(string terrainTypeKey) : this(TerrainType.TYPES[terrainTypeKey])
        {
            Contract.Requires<ArgumentNullException>(terrainTypeKey != null);
        }

        private Terrain(TerrainType type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            var attributes = new IAttribute[type.Attributes.Count];

            var i = 0;
            foreach (var attr in type.Attributes)
            {
                attributes[i++] = attr.Definition.GetAttribute(attr.GetValue(), false);
            }

            attributeContainer = new AttributeContainer(attributes);
        }

        [Newtonsoft.Json.JsonConstructor]
        public Terrain(IAttribute[] attributes)
        {
            attributeContainer = new AttributeContainer(TerrainType.TERRAIN_ATTRIBUTES_BUILDER.BuildFullAttributeList(attributes));
        }
    }
}
