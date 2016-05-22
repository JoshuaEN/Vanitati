using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UnnamedStrategyGame.UI.TileUI.Terrain
{
    public class DestroyedRoad : Road
    {
        public override Game.BaseType Type { get; } = Game.TerrainTypes.DestroyedRoad.Instance;

        private DestroyedRoad() { }
        public static new DestroyedRoad Instance { get; } = new DestroyedRoad();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            return GenerateDestroyedVersion(base.RenderVisualization(height, width, background, Brushes.BurlyWood, outline), height, width);
        }
    }
}
