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
    public class Airfield : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Airfield.Instance;

        protected Airfield() { }
        public static Airfield Instance { get; } = new Airfield();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();

            var top = height * 0.4;
            var tower_height = height - top;
            var left = width * 0.25;
            var tower_width = width * 0.2;

            var top_tower_height = tower_height * 0.25;
            var top_tower_top = top;

            var step = tower_height * 0.25;

            var path = new PathGeometry();

            var poly =
                new PathFigure(
                    new Point(left, top),
                        new List<PathSegment>() {
                            new LineSegment(new Point(left, top), true),
                            new LineSegment(new Point(left, top + top_tower_height), true),
                            new LineSegment(new Point(left +  tower_width / 4, top + top_tower_height), true),
                            new LineSegment(new Point(left + tower_width / 4, top + tower_height), true),
                            new LineSegment(new Point(left + tower_width * 0.75, top + tower_height), true),
                            new LineSegment(new Point(left + tower_width * 0.75, top + top_tower_height), true),
                            new LineSegment(new Point(left + tower_width, top + top_tower_height), true),
                            new LineSegment(new Point(left + tower_width, top), true),
                    },
                true
            );


            path.Figures.Add(poly);

            var poly2 =
                new PathFigure(
                    new Point(left + tower_width, height),
                    new List<PathSegment>()
                    {
                        new ArcSegment(new Point(left + tower_width + width * 0.55, height), new Size(width * 0.4, height * 0.7), 0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(left + tower_width + width * 0.50, height), true),
                        new ArcSegment(new Point(left + tower_width + width * 0.05, height), new Size(width * 0.37, height * 0.7), 0, false, SweepDirection.Counterclockwise, true),
                    },
                    true
                );

            path.Figures.Add(poly2);

            group.Children.Add(new GeometryDrawing(highlight, outline, path));

            group.Children.Add(new GeometryDrawing(background, outline, new RectangleGeometry(new Rect(left + tower_width * 0.1, top + top_tower_height * 0.2, tower_width * 0.4, top_tower_height * 0.6))));
            group.Children.Add(new GeometryDrawing(background, outline, new RectangleGeometry(new Rect(left + tower_width * 0.5, top + top_tower_height * 0.2, tower_width * 0.4, top_tower_height * 0.6))));

            return group;
        }
    }
}
