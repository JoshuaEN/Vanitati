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
    public class Runway : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Runway.Instance;

        private Runway() { }
        public static Runway Instance { get; } = new Runway();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var top = 0;
            var left = 0;

            var group = new DrawingGroup();
            group.Append();

            var path = new PathGeometry();

            var fourthWidth = width / 6;

            var poly =
                new PathFigure(
                    new Point(fourthWidth, height),
                        new List<PathSegment>() {
                        new LineSegment(new Point(fourthWidth * 2, 0), true),
                        new LineSegment(new Point(fourthWidth * 4, 0), true),
                        new LineSegment(new Point(fourthWidth * 5, height), true)
                    },
                true
            );

            path.Figures.Add(poly);


            group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(46, 43, 43)), outline, path));

            var groupb = new DrawingGroup();

            path = new PathGeometry();

            var width_step = (fourthWidth * 5 - fourthWidth) / 12;

            var step = 1.0;
            var line_height = height * 0.8;
            var width_skew = fourthWidth * 0.0;

            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 2 + width_step * step, height), new Point(fourthWidth * 2 + width_step * step, 0))); step += 1.5;
            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 2 + width_step * step, height), new Point(fourthWidth * 2 + width_step * step, 0))); step += 1.5;
            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 2 + width_step * step, height), new Point(fourthWidth * 2 + width_step * step, 0))); step += 4;
            path.Transform = new SkewTransform(-10, 0);
            groupb.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(highlight, 6), path));


            path = new PathGeometry();
            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 0 + width_step * step, height), new Point(fourthWidth * 0 + width_step * step, 0))); step += 1.5;
            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 0 + width_step * step, height), new Point(fourthWidth * 0 + width_step * step, 0))); step += 1.5;
            path.AddGeometry(new LineGeometry(new Point(fourthWidth * 0 + width_step * step, height), new Point(fourthWidth * 0 + width_step * step, 0))); step += 1;

            path.Transform = new SkewTransform(10, 0);

            groupb.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(highlight, 6), path));

            groupb.ClipGeometry = new RectangleGeometry(new Rect(0, height * 0.8, width, height * 0.2));

            group.Children.Add(groupb);

            return group;
        }
    }
}
