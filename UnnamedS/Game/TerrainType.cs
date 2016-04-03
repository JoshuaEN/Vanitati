using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Attribute;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public abstract class TerrainType : BaseType, IAttributeContainer
    {
        public IReadOnlyList<UnitType> Buildable { get; }

        private readonly AttributeContainer attributeContainer;

        protected TerrainType(
            string key, 
            Dictionary<string, object> attributeDefaults, 
            List<UnitType> buildable = null) : base("terrain_" + key)
        {
            if(buildable == null)
            {
                Buildable = new List<UnitType>(0);
            }
            else
            {
                Buildable = buildable;
            }

            attributeDefaults.Add(TYPE, this);
            attributeContainer = new AttributeContainer(TERRAIN_ATTRIBUTES_BUILDER.BuildFullAttributeList(attributeDefaults, true));
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

        protected const string PREFIX = "terrain_attr_";
        public const string CAPTURE_POINTS = PREFIX + "capture_points";
        public const string TYPE = PREFIX + "type";
        public const string LOCATION = PREFIX + "location";

        protected static readonly AttributeDefinition<TerrainType> TYPE_DEF = new AttributeDefinition<TerrainType>(TYPE);
        protected static readonly AttributeDefinition<int> CAPTURE_POINTS_DEF = new AttributeDefinition<int>(CAPTURE_POINTS, 10, new List<AttributeValidator>()
                {
                    new Attribute.Validator.NumberBetween<int>(0, 10)
                });
        protected static readonly AttributeDefinition<Location> LOCATION_DEF = new AttributeDefinition<Location>(LOCATION, new Location());

        public static readonly AttributeBuilder TERRAIN_ATTRIBUTES_BUILDER = new AttributeBuilder(TYPE_DEF, LOCATION_DEF, CAPTURE_POINTS_DEF);

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

        public static IReadOnlyDictionary<string, TerrainType> TYPES { get; }

        public IReadOnlyList<IAttribute> Attributes
        {
            get
            {
                return attributeContainer.Attributes;
            }
        }

        static TerrainType()
        {
            TYPES = BuildTypeListing<TerrainType>("UnnamedStrategyGame.Game.TerrainTypes");
        }
    }
}
