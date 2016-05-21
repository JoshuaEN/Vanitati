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
    public class Factory : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Factory.Instance;

        private Factory() { }
        public static Factory Instance { get; } = new Factory();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();

            var top = height * 0.6;
            var factory_height = height - top;
            var left = width * 0.3;
            var factory_width = width * 0.55;

            var step = factory_width / 5;

            var path = new PathGeometry();

            var poly = 
                new PathFigure(
                    new Point(left, top), 
                        new List<PathSegment>() {
                        new LineSegment(new Point(left + step, top + step), true),
                        new LineSegment(new Point(left + step, top), true),
                        new LineSegment(new Point(left + step * 2, top + step), true),
                        new LineSegment(new Point(left + step * 2, top), true),
                        new LineSegment(new Point(left + step * 3, top + step), true),
                        new LineSegment(new Point(left + factory_width - 18, top + step), true),
                        new LineSegment(new Point(left + factory_width - 18, top - step * 1.5), true),
                        new LineSegment(new Point(left + factory_width - 13, top - step * 1.5), true),
                        new LineSegment(new Point(left + factory_width - 13, top + step), true),
                        new LineSegment(new Point(left + factory_width - 10, top + step), true),
                        new LineSegment(new Point(left + factory_width - 10, top - step * 1.5), true),
                        new LineSegment(new Point(left + factory_width - 5, top - step * 1.5), true),
                        new LineSegment(new Point(left + factory_width - 5, top + step), true),
                        new LineSegment(new Point(left + factory_width, top + step), true),
                        new LineSegment(new Point(left + factory_width, top + factory_height), true),
                        new LineSegment(new Point(left, top + factory_height), true)
                    },
                true          
            );


            path.Figures.Add(poly);

            

            group.Children.Add(new GeometryDrawing(highlight, outline, path));

            return group;
            
        }
    }
}
