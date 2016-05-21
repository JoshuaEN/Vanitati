using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI.TileUI
{
    public abstract class TerrainBase : BaseUI
    {
        public virtual Brush BackgroundFill { get; } = new SolidColorBrush(Color.FromRgb(152, 161, 153));

        public Drawing GetVisualization(double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(new GeometryDrawing(BackgroundFill, new Pen(null, 0), new RectangleGeometry(new Rect(0, 0, width, height))));
            //group.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Transparent, 1.0), new LineGeometry(new Point(0, 0), new Point(width, height))));
            group.Children.Add(RenderVisualization(height, width, background, highlight, outline));
            return group;
        }

        protected abstract Drawing RenderVisualization(double height, double width, Brush background, Brush highlight, Pen outline);

        public static Dictionary<BaseType, TerrainBase> TYPES { get; }

        static TerrainBase()
        {
            TYPES = BuildTypeListing("UnnamedStrategyGame.UI.TileUI.Terrain");
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static Dictionary<BaseType, TerrainBase> BuildTypeListing(string nameSpace)
        {
            var listing = new Dictionary<BaseType, TerrainBase>();

            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass && typeof(TerrainBase).IsAssignableFrom(t) && t.IsAbstract == false && t.Namespace.StartsWith(nameSpace)
                        select t;

            foreach (var t in types)
            {
                //if (t.IsSealed == false)
                //{
                //    throw new Exceptions.InvalidDefinitionException(String.Format("Game Type {0} must be declared as sealed", t));
                //}

                var prop = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                if (prop == null)
                {
                    throw new ArgumentException(String.Format("Game Type {0} must declare a public static property Instance", t));
                }

                var res = (TerrainBase)prop.GetValue(null);

                listing.Add(res.Type, res);
            }
            return listing;
        }
    }
}
