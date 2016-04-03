using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame
{
    public static class Globals
    {
        public static int DEFAULT_PORT { get; } = 4000;

        public static string ATTRIBUTE_NAME_UNIT_TYPE { get; } = "type";
        public static string ATTRIBUTE_NAME_TERRAIN_TYPE { get; } = "type";

        public static int UNIQUE_PLAYER_ID_RESERVED_NAMESPACE_END { get; } = 100;
        public static int UNIQUE_PLAYER_ID_NONE { get; }  = 0;


        public static string RESOURCE_PATH { get; } = System.IO.Path.Combine("Resources", "vanilla");
        public static string RESOURCE_IMAGE_PATH { get; } = System.IO.Path.Combine(RESOURCE_PATH, "images");
    }
}
