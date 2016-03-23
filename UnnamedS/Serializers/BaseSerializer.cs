using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Serializers
{
    public abstract class BaseSerializer
    {
        public abstract string Serialize(object obj);
        public abstract T Deserialize<T>(string str);
    }
}
