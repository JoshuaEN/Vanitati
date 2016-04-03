using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public class Player : IAttributeContainer
    {
        public int UniqueId { get; }

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

        public Player(int uniqueId)
        {
            UniqueId = uniqueId;
            attributeContainer = new AttributeContainer(ATTRIBUTES_BUILDER.BuildFullAttributeList(new Dictionary<string, object>()
            {
                {UNIQUE_ID, uniqueId }
            }, false));

            SetAttributeReadOnly(UNIQUE_ID);
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

        protected const string CREDITS = "credits";
        protected const string UNIQUE_ID = "player_id";

        protected static readonly AttributeDefinition<int> UNIQUE_ID_DEF = new AttributeDefinition<int>(UNIQUE_ID);
        protected static readonly AttributeDefinition<int> CREDITS_DEF = new AttributeDefinition<int>(CREDITS, 0);

        public static readonly AttributeBuilder ATTRIBUTES_BUILDER = new AttributeBuilder(UNIQUE_ID_DEF, CREDITS_DEF);
    }
}
