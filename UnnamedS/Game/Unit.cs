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
        [Newtonsoft.Json.JsonIgnore]
        public int UniqueId
        {
            get;
        }

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

        public Unit(int unitId, string unitTypeKey, int player_id) : this(unitId, UnitType.TYPES[unitTypeKey], player_id)
        {
            Contract.Requires<ArgumentNullException>(unitTypeKey != null);
        }

        private Unit(int unitId, UnitType type, int player_id)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            var attributes = new IAttribute[type.Attributes.Count];

            var i = 0;
            foreach(var attr in type.Attributes)
            {
                object value;
                bool readOnly = false;

                if (attr.Key == UnitType.PLAYER_ID)
                {
                    value = player_id;
                }
                else if (attr.Key == UnitType.UNIT_ID)
                {
                    value = unitId;
                    readOnly = true;
                }
                else
                {
                    value = attr.GetValue();
                }

                attributes[i++] = attr.Definition.GetAttribute(value, readOnly);
            }

            UniqueId = unitId;
            attributeContainer = new AttributeContainer(attributes);
        }

        public Unit(IReadOnlyList<IAttribute> attributes)
        {
            attributeContainer = new AttributeContainer(UnitType.UNIT_ATTRIBUTES_BUILDER.BuildFullAttributeList(attributes.ToArray(), false));

            UniqueId = (int)GetAttribute(UnitType.UNIT_ID).GetValue();
        }

    }
}
