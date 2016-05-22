using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI.TileUI.Terrain
{
    public class DestroyedFactory : Factory
    {
        public override BaseType Type { get; } = Game.TerrainTypes.DestroyedFactory.Instance;

        private DestroyedFactory() { }
        public static new DestroyedFactory Instance { get; } = new DestroyedFactory();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            return GenerateDestroyedVersion(base.RenderVisualization(height, width, background, Brushes.BurlyWood, outline), height, width);
        }
    }
}
