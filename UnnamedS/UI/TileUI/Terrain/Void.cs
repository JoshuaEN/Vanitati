using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI.TileUI.Terrain
{
    public class Void : TerrainUI
    {
        public override BaseType Type { get; } = Tile.Void.Instance;

        private Void() { }
        public static Void Instance { get; } = new Void();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            return group;
        }
    }
}
