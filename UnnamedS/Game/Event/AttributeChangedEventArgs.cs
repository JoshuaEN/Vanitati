using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public abstract class AttributeChangedEventArgs : EventArgs
    {
        public Type Type { get; }
        public string Key { get; }

        protected AttributeChangedEventArgs(string key, Type t)
        {
            Type = t;
            Key = key;
        }
    }

    public class AttributeChangedEventArgs<T> : AttributeChangedEventArgs
    {      
        public T OldValue { get; }
        public T NewValue { get; }

        public AttributeChangedEventArgs(string key, T oldValue, T newValue) : base(key, typeof(T))
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public interface INotifyAttributeChanged
    {
        event EventHandler<AttributeChangedEventArgs> AttributeChanged;
    }
}
