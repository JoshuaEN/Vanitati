using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public static class Globals
    {
        public static string ATTRIBUTE_NAME_UNIT_TYPE { get; } = "type";
        public static string ATTRIBUTE_NAME_TERRAIN_TYPE { get; } = "type";

        public static uint UNIQUE_PLAYER_ID_RESERVED_NAMESPACE_END { get; } = 100;
        public static uint UNIQUE_PLAYER_ID_NONE { get; }  = 0;
    }
}
