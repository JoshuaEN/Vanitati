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
    [ContractClass(typeof(ContractClassForIAttributeContainer))]
    public interface IAttributeContainer : INotifyPropertyChanged, INotifyAttributeChanged
    {
        IReadOnlyList<IAttribute> Attributes
        {
            get;
        }

        IAttribute GetAttribute(string key);
        void SetAttribute(IAttribute value);
        void SetAttributeReadOnly(string key);
        void SetAttributes(IReadOnlyList<IAttribute> values);
    }

    [ContractClassFor(typeof(IAttributeContainer))]
    internal abstract class ContractClassForIAttributeContainer : IAttributeContainer
    {
        public abstract IReadOnlyList<IAttribute> Attributes { get; }

        public abstract event EventHandler<AttributeChangedEventArgs> AttributeChanged;
        public abstract event PropertyChangedEventHandler PropertyChanged;

        public IAttribute GetAttribute(string key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            return null;
        }

        public void SetAttribute(IAttribute value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
        }

        public void SetAttributeReadOnly(string key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
        }

        public void SetAttributes(IReadOnlyList<IAttribute> values)
        {
            Contract.Requires<ArgumentNullException>(values != null);
        }
    }
}
