using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI.TileUI.Terrain
{
    public class City : TerrainUI
    {
        public override BaseType Type { get; } = Game.TerrainTypes.City.Instance;

        private City() { }
        public static City Instance { get; } = new City();

        protected override Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(CreateBuilding(height * 0.5, 0, height * 0.5, width / 3.0, background, highlight, outline));
            group.Children.Add(CreateBuilding(height * 0.3, width * 0.5, height * 0.7, width / 6.0, background, highlight, outline));
            return group;
        }

        private Drawing CreateBuilding(double top, double left, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();

            group.Append();
            group.Children.Add(new GeometryDrawing(highlight, outline, new RectangleGeometry(new Rect(left, top, width, height))));
            
            var path = new PathGeometry();

            var window_top = top;
            var window_left = left;
            var windows_x = 3;
            var windows_y = 5;
            var window_width = (width / ((windows_x * 2) + 1));
            var window_height = (height / ((windows_y * 2) + 1));

            for (var x = 0; x < windows_x; x++)
            {
                for(var y = 0; y < windows_y; y++)
                {
                    path.AddGeometry(new RectangleGeometry(new Rect(window_left + window_width * (x * 2 + 1), window_top + window_height * (y * 2 + 1), window_width, window_height)));
                }
            }

            group.Children.Add(new GeometryDrawing(background, outline, path));
            return group;
        }

    }
}
