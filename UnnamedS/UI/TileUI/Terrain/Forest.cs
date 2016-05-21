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
    public class Forest : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Forest.Instance;

        private Forest() { }
        public static Forest Instance { get; } = new Forest();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(CreateTree(height * 0.4, width * 0.45, height * 0.6, width * 0.2, 0.25, background, highlight, outline));
            group.Children.Add(CreateTree(height * 0.5, width * 0.25, height * 0.5, width * 0.3, 0.15, background, highlight, outline));

            return group;
        }

        private Drawing CreateTree(double top, double left, double height, double width, double dist_from_group_mod, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();

            var path = new PathGeometry();

            var dist_from_ground = height * dist_from_group_mod;
            var trunk_half_thickness = width * 0.10;

            var poly =
                new PathFigure(
                    new Point(left, top + height - dist_from_ground),
                        new List<PathSegment>() {
                        new LineSegment(new Point(left + width / 2, top), true),
                        new LineSegment(new Point(left + width, top + height - dist_from_ground), true),
                        new LineSegment(new Point(left + width / 2 + trunk_half_thickness, top + height - dist_from_ground), true),
                        new LineSegment(new Point(left + width / 2 + trunk_half_thickness, top + height), true),
                        new LineSegment(new Point(left + width / 2 - trunk_half_thickness, top + height), true),
                        new LineSegment(new Point(left + width / 2 - trunk_half_thickness, top + height - dist_from_ground), true),
                    },
                true
            );


            path.Figures.Add(poly);



            group.Children.Add(new GeometryDrawing(Brushes.DarkGreen, outline, path));

            return group;
        }
    }
}
