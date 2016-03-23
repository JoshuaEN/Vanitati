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
    public sealed class Unit : IAttributeContainer
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

        public Unit(string unitTypeKey, uint player_id) : this(UnitType.TYPES[unitTypeKey], player_id)
        {
            Contract.Requires<ArgumentNullException>(unitTypeKey != null);
        }

        private Unit(UnitType type, uint player_id)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            var attributes = new IAttribute[type.Attributes.Count];

            var i = 0;
            foreach(var attr in type.Attributes)
            {
                object value;
                if (attr.Key == "player_id")
                    value = player_id;
                else
                    value = attr.GetValue();

                attributes[i++] = attr.Definition.GetAttribute(value, false);
            }

            attributeContainer = new AttributeContainer(attributes);
        }

    }
}
