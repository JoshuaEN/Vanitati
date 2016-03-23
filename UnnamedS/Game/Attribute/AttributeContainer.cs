using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public class AttributeContainer : IAttributeContainer
    {
        private Dictionary<string, IAttribute> _attributes { get; }

        public IReadOnlyList<IAttribute> Attributes
        {
            get;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<AttributeChangedEventArgs> AttributeChanged;

        public IAttribute GetAttribute(string key)
        {
            return _attributes[key];
        }

        public void SetAttribute(IAttribute value)
        {
            _attributes[value.Key].SetValue(value);
        }

        public AttributeContainer(params IAttribute[] attributes)
        {
            Contract.Requires<ArgumentNullException>(attributes != null);

            _attributes = new Dictionary<string, IAttribute>();

            foreach(var attr in attributes)
            {
                _attributes.Add(attr.Key, attr);
                attr.PropertyChanged += AttributePropertyChanged;
            }

            Attributes = _attributes.Values.ToList().AsReadOnly();
        }

        private void AttributePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        public AttributeContainer(IList<IAttribute> attributes) : this(attributes.ToArray())
        {
            Contract.Requires<ArgumentNullException>(attributes != null);
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected void OnAttributeChanged(AttributeChangedEventArgs e)
        {
            if(AttributeChanged != null)
            {
                AttributeChanged(this, e);
            }
        }

        public void SetAttributeReadOnly(string key)
        {
            GetAttribute(key).MakeReadOnly();
        }

        public void SetAttributes(IReadOnlyList<IAttribute> values)
        {
            foreach(var v in values)
            {
                SetAttribute(v);
            }
        }
    }
}
