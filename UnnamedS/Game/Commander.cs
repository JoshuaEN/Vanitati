using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;

namespace UnnamedStrategyGame.Game
{
    public class Commander : IPropertyContainer
    {
        public int CommanderID { get; } = -1;
        public int Credits { get; set; } = -1;

        [Newtonsoft.Json.JsonConstructor]
        public Commander(int commanderID, int credits = 0)
        {
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

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(CommanderID >= -1);
            Contract.Invariant(Credits >= -1);
        }
    }
}
