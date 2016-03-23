using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using UnnamedStrategyGame.Game.Event;
using System.ComponentModel;
using Newtonsoft.Json;

namespace UnnamedStrategyGame.Game
{
    public sealed class Attribute<T> : IAttribute
    {
        public string Key { get { return Definition.Key; } }

        [JsonProperty(Order = 2)]
        public T Value { get; private set; }

        [JsonProperty(Order = 1)]
        public IAttributeDefinition Definition { get; }

        [JsonProperty(Order = 3)]
        public bool ReadOnly { get; private set; }

        public Attribute(IAttributeDefinition definition, T value, bool readOnly = false)
        {
            Contract.Requires<ArgumentNullException>(definition != null);
            Contract.Requires<ArgumentNullException>(value != null);

            Definition = definition;
            Contract.Assume(Definition != null);
            Value = value;
            Definition.ValidateAttributeValue(this);
            ReadOnly = readOnly;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<AttributeChangedEventArgs> AttributeChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnAttributeChanged(AttributeChangedEventArgs<T> e)
        {
            var handler = AttributeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SetValue(IAttribute source)
        {
            if(ReadOnly)
            {
                throw new Exceptions.AttributeReadOnlyException();
            }
            var oldValue = Value;
            Value = (source as Attribute<T>).Value;

            if (oldValue.Equals(Value) == false)
            {
                OnAttributeChanged(new AttributeChangedEventArgs<T>(Key, oldValue, Value));
                OnPropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }

        public object GetValue()
        {
            return Value;
        }

        public void MakeReadOnly()
        {
            ReadOnly = true;
        }
    }
}
