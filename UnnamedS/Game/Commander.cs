using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Commander : IPropertyContainer
    {
        public CommanderType CommanderType { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        private int _commanderID = -1;
        public int CommanderID
        {
            get { return _commanderID; }
            set
            {
                Contract.Requires<ArgumentException>(value >= 0);
                if (_commanderID != -1)
                    throw new NotSupportedException("Commander ID cannot be changed after being set");

                _commanderID = value;
            }
        }
        public int Credits { get; private set; } = -1;

        [Newtonsoft.Json.JsonConstructor]
        public Commander(CommanderType commanderType, int commanderID, int credits = 0)
        {
            Contract.Requires<ArgumentNullException>(null != commanderType);

            CommanderType = commanderType;
            CommanderID = commanderID;
            Credits = credits;
        }

        public Commander(IDictionary<string, object> values)
        {
            SetProperties(values);
        }

        public IDictionary<string, object> GetProperties()
        {
            return PropertyContainer.COMMANDER.GetProperties(this);
        }

        public IDictionary<string, object> GetWriteableProperties()
        {
            return PropertyContainer.COMMANDER.GetWriteableProperties(this);
        }

        public void SetProperties(IDictionary<string, object> values)
        {
            PropertyContainer.COMMANDER.SetProperties(this, values);
        }

        public override string ToString()
        {
            return $"{CommanderType}: #{CommanderID}, ${Credits}";
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(null != CommanderType);
            Contract.Invariant(CommanderID >= -1);
            Contract.Invariant(Credits >= -1);
        }
    }
}
