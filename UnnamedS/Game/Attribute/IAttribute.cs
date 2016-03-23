using Newtonsoft.Json;
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
    [ContractClass(typeof(ContractClassForIAttribute))]
    public interface IAttribute : INotifyPropertyChanged, Event.INotifyAttributeChanged
    {

        IAttributeDefinition Definition { get; }
        /// <summary>
        /// The Key for identifying this attribute.
        /// </summary>
        [JsonIgnore]
        string Key { get; }
        bool ReadOnly { get; }

        /// <summary>
        /// Sets the value of this attribute to the value of the given Source.
        /// </summary>
        /// <param name="source">The source of the value to set.</param>
        void SetValue(IAttribute source);

        object GetValue();
        void MakeReadOnly();
    }

    [ContractClassFor(typeof(IAttribute))]
    internal abstract class ContractClassForIAttribute : IAttribute
    {
        public IAttributeDefinition Definition { get; }
        public abstract string Key { get; }
        public abstract bool ReadOnly { get; }
        public abstract Type Type { get; }

        public abstract event EventHandler<AttributeChangedEventArgs> AttributeChanged;
        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract object GetValue();
        public abstract void MakeReadOnly();

        public void SetValue(IAttribute source)
        {
            Contract.Requires<Exceptions.IncompatableAttributeException>(Definition.CheckAttribute(source), "Incompatable Attribute");
            Contract.Requires(Definition.ValidateAttributeValue(source));
        }
    }
}
