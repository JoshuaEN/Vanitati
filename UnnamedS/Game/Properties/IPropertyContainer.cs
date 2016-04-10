using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Properties
{
    public interface IPropertyContainer
    {
        IDictionary<string, object> GetProperties();
        IDictionary<string, object> GetWriteableProperties();
        void SetProperties(IDictionary<string, object> values);
    }
}
