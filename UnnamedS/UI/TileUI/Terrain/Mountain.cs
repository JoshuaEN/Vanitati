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
    public class Mountain : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Mountain.Instance;

        private Mountain() { }
        public static Mountain Instance { get; } = new Mountain();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(CreateMountain(height * 0.50, width * 0.3, height * 0.50, width * 0.4, background, highlight, outline));
            group.Children.Add(CreateMountain(height * 0.70, width * 0.2, height * 0.30, width * 0.25, background, highlight, outline));

            return group;
        }
        private Drawing CreateMountain(double top, double left, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();

            var path = new PathGeometry();

            var poly =
                new PathFigure(
                    new Point(left, top + height),
                        new List<PathSegment>() {
                        new LineSegment(new Point(left + width / 2, top), true),
                        new LineSegment(new Point(left + width, top + height), true)
                    },
                true
            );


            path.Figures.Add(poly);



            group.Children.Add(new GeometryDrawing(Brushes.DimGray, outline, path));

            return group;
        }
    }
}
