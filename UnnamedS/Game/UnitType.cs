using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Attribute;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public abstract class UnitType : BaseType, IAttributeContainer
    {
        public const string PREFIX = "unit_attr_";
        public const string TYPE = PREFIX + "type";
        public const string UNIT_ID = PREFIX + "unit_id";
        public const string PLAYER_ID = PREFIX + "player_id";
        public const string LOCATION = PREFIX + "location";
        public const string MOVEMENT = PREFIX + "movement";
        public const string HEALTH = PREFIX + "health";

        protected static readonly AttributeDefinition<UnitType> TYPE_DEF = new AttributeDefinition<UnitType>(TYPE);
        protected static readonly AttributeDefinition<int> UNIT_ID_DEF = new AttributeDefinition<int>(UNIT_ID, -1);
        protected static readonly AttributeDefinition<int> PLAYER_ID_DEF = new AttributeDefinition<int>(PLAYER_ID, Globals.UNIQUE_PLAYER_ID_NONE);
        protected static readonly AttributeDefinition<Location> LOCATION_DEF = new AttributeDefinition<Location>(LOCATION, new Location());
        protected static readonly AttributeDefinition<int> MOVEMENT_DEF = new AttributeDefinition<int>(MOVEMENT);
        protected static readonly AttributeDefinition<int> HEALTH_DEF = new AttributeDefinition<int>(HEALTH, 10, new List<AttributeValidator>()
                {
                    new Attribute.Validator.NumberBetween<int>(0, 10)
                });

        public static readonly AttributeBuilder UNIT_ATTRIBUTES_BUILDER = new AttributeBuilder(TYPE_DEF, PLAYER_ID_DEF, LOCATION_DEF, MOVEMENT_DEF, HEALTH_DEF);


        public static IReadOnlyDictionary<string, UnitType> TYPES { get; }

        static UnitType()
        {
            TYPES = BuildTypeListing<UnitType>("UnnamedStrategyGame.Game.UnitTypes");
        }

        public IReadOnlyList<IAttribute> Attributes
        {
            get { return attributeContainer.Attributes; }
        }

        public MovementType MovementType { get; }
        public IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; }
        public IReadOnlyList<ActionType> Actions { get; }

        private readonly AttributeContainer attributeContainer;

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

        protected UnitType(
            string key,
            Dictionary<string, object> attributeDefaults,
            MovementType movementType, 
            List<ActionType> actions, 
            Dictionary<SupplyType, int> supplyLimits) : base("unit_" + key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(attributeDefaults != null);
            Contract.Requires<ArgumentNullException>(movementType != null);
            Contract.Requires<ArgumentNullException>(actions != null);
            Contract.Requires<ArgumentNullException>(supplyLimits != null);

            MovementType = movementType;
            Actions = actions;
            SupplyLimits = supplyLimits;

            // Inject the TYPE instance into the attributes.
            attributeDefaults.Add(TYPE, this);
            attributeContainer = new AttributeContainer(UNIT_ATTRIBUTES_BUILDER.BuildFullAttributeList(attributeDefaults, true));
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
    }
}
