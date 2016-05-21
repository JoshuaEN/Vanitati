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
    public class Plain : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.Plain.Instance;

        private Plain() { }
        public static Plain Instance { get; } = new Plain();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(CreateHill(height * 0.60, 0, height * 0.40, width, background, highlight, outline));

            return group;
        }
        private Drawing CreateHill(double top, double left, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();

            var path = new PathGeometry();

            var poly =
                new PathFigure(
                    new Point(left, top + height),
                        new List<PathSegment>() {
                        new ArcSegment(new Point(left + width, top + height), new Size(width / 2, height / 3.5), 0, false, SweepDirection.Clockwise, true)
                    },
                true
            );


            path.Figures.Add(poly);



            group.Children.Add(new GeometryDrawing(Brushes.MediumSeaGreen, outline, path));

            return group;
        }
    }
}
