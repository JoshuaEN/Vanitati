using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.UI.Settings
{
    public class Settings
    {
        public static Settings Current { get; } = new Settings();
        public InputBindings InputBindings { get; set; } = new InputBindings();
    }
}
