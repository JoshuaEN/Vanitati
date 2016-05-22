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
        private Brush FontForeground { get; } = Brushes.AntiqueWhite;
        private Brush FontBackground { get; } = Brushes.Black;
        public virtual Brush BackgroundFill { get; } = new SolidColorBrush(Color.FromRgb(152, 161, 153));

        public Drawing GetVisualization(Game.Terrain terrain, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(new GeometryDrawing(BackgroundFill, new Pen(null, 0), new RectangleGeometry(new Rect(0, 0, width, height))));
            //group.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Transparent, 1.0), new LineGeometry(new Point(0, 0), new Point(width, height))));
            group.Children.Add(RenderVisualization(height, width, background, highlight, outline));

            if (terrain.Health < terrain.TerrainType.MaxHealth && terrain.TerrainType.CanBePillage == true)
                group.Children.Add(GetHealthText(terrain, height, width, background, highlight, outline));

            if(terrain.DigIn > 0)
                group.Children.Add(GetDigInText(terrain, height, width, background, highlight, outline));

            return group;
        }

        private Drawing GetDigInText(Game.Terrain terrain, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var dGroup = new DrawingGroup();
            var font = new FormattedText($"{terrain.DigIn}", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Lucida Console"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 10, FontForeground);

            var padding = 1;
            var fontTop = 0 + padding;// - font.Height;
            var fontLeft = width / 4 + padding;// - font.Width;
            var geo = font.BuildGeometry(new Point(fontLeft, fontTop));

            dGroup.Children.Add(new GeometryDrawing(FontBackground, new Pen(FontBackground, 0), new RectangleGeometry(new Rect(fontLeft - padding, fontTop - padding, font.Width + padding * 2, font.Height + padding * 2))));
            dGroup.Children.Add(new GeometryDrawing(FontForeground, new Pen(FontForeground, 0), geo));


            return dGroup;
        }

        private Drawing GetHealthText(Game.Terrain terrain, double height, double width, Brush background, Brush highlight, Pen outline)
        {
            var dGroup = new DrawingGroup();
            var font = new FormattedText(terrain.Health.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Lucida Console"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 10, FontForeground);

            var padding = 1;
            var fontTop = 0 + padding;// - font.Height;
            var fontLeft = (width / 4) * 3 - padding - font.Width;
            var geo = font.BuildGeometry(new Point(fontLeft, fontTop));

            dGroup.Children.Add(new GeometryDrawing(FontBackground, new Pen(FontBackground, 0), new RectangleGeometry(new Rect(fontLeft - padding, fontTop - padding, font.Width + padding * 2, font.Height + padding * 2))));
            dGroup.Children.Add(new GeometryDrawing(FontForeground, new Pen(FontForeground, 0), geo));


            return dGroup;
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

        protected Drawing GenerateDestroyedVersion(Drawing original, double height, double width)
        {
            var drawingGroup = new DrawingGroup();
            drawingGroup.Append();
            drawingGroup.Children.Add(GenerateFire(height, width));
            drawingGroup.Children.Add(original);

            drawingGroup.Freeze();
            return drawingGroup;
        }

        protected Drawing GenerateFire(double height, double width)
        {
            var top = height * 0.7;
            var bottom = height * 0.8;
            var left = 0;

            var path = new PathGeometry();

            var segs = new List<PathSegment>();

            var min = 0;
            var max = height * 0.9;
            var range = max - min;

            var progress = 0.0;

            var rnd = new Random(1111);

            while (progress <= width)
            {
                segs.Add(new LineSegment(new Point(progress, min + rnd.NextDouble() * range), true));

                progress += width * 0.05;
            }

            segs.Add(new LineSegment(new Point(width, height), true));

            var poly =
                new PathFigure(
                    new Point(0, height),
                        segs,
                true
            );

            path.Figures.Add(poly);

            var brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Colors.Red, 0.05),
                    new GradientStop(Colors.OrangeRed, 0.10),
                    new GradientStop(Colors.Orange, 0.15),
                    new GradientStop(Colors.Yellow, 0.20),
                    new GradientStop(Colors.Transparent, 0.35)
                }, 270);
            brush.Opacity = 0.5;
            path.FillRule = FillRule.Nonzero;
            return new GeometryDrawing(brush, new Pen(Brushes.DarkRed, 1.0), path);
        }
    }
}
