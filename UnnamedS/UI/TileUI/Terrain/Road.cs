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
    public class Road : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Road.Instance;

        private Road() { }
        public static Road Instance { get; } = new Road();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var top = 0;
            var left = 0;

            var group = new DrawingGroup();
            group.Append();

            var path = new PathGeometry();

            var fourthWidth = width / 4;

            var poly =
                new PathFigure(
                    new Point(fourthWidth, height),
                        new List<PathSegment>() {
                        new LineSegment(new Point(fourthWidth * 2, 0), true),
                        new LineSegment(new Point(fourthWidth * 2, 0), true),
                        new LineSegment(new Point(fourthWidth * 3, height), true)
                    },
                true
            );

            path.Figures.Add(poly);

            //path.AddGeometry(new LineGeometry(new Point(fourthWidth, height), new Point(fourthWidth * 2, 0)));
            //path.AddGeometry(new LineGeometry(new Point(fourthWidth * 3, height), new Point(fourthWidth * 2, 0)));

            var previous_dash_y_end = 0.0;

            var middle_dash_length = height / 10.0;

            while(previous_dash_y_end < height)
            {
                var start_drawing_y = previous_dash_y_end + middle_dash_length;
                var end_drawing_y = previous_dash_y_end + middle_dash_length * 2;

                if (start_drawing_y >= height)
                    break;

                if (end_drawing_y >= height)
                    end_drawing_y = height;

                path.AddGeometry(new LineGeometry(new Point(fourthWidth * 2, start_drawing_y), new Point(fourthWidth * 2, end_drawing_y)));

                previous_dash_y_end += middle_dash_length * 2;
                middle_dash_length = middle_dash_length * 1.5;
            }

            group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(46, 43, 43)), new Pen(Brushes.WhiteSmoke, 1.0), path));

            return group;
        }
    }
}
