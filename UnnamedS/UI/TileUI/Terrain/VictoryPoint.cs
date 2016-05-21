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
    public class VictoryPoint : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.VictoryPoint.Instance;

        private VictoryPoint() { }
        public static VictoryPoint Instance { get; } = new VictoryPoint();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var path = new PathGeometry();
            var segs = new List<Point>();

            double rx = width / 2.5;
            double ry = height / 2.5;
            double cx = rx * 1.25;
            double cy = ry * 1.25;

            var num_points = 5;

            double theta = -Math.PI / 2;
            double dtheta = 4 * Math.PI / num_points;
            for(var i = 0; i < num_points; i++)
            {
                segs.Add(new Point(cx + rx * Math.Cos(theta), cy + ry * Math.Sin(theta)));

                theta += dtheta;
            }

            var firstPoint = segs[0];
            segs.RemoveAt(0);

            var poly = new PathFigure(firstPoint, segs.Select(p => new LineSegment(p, true)), true);


            path.Figures.Add(poly);

            path.FillRule = FillRule.Nonzero;
            return new GeometryDrawing(highlight, outline, path);
        }
    }
}
