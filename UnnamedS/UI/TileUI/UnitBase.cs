using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.UnitTypes;

namespace UnnamedStrategyGame.UI.TileUI
{
    public class UnitBase : BaseUI
    {

        static UnitBase()
        {
            new UnitBase(AirTransport.Instance, Units.FixedWingAircraft | Units.Transportation, Modifiers.None);
            new UnitBase(Bomber.Instance, Units.FixedWingAircraft, Modifiers.None);

            new UnitBase(AntiTankInfantry.Instance, Units.Infantry | Units.AntiTank, Modifiers.Rocket);
            new UnitBase(Infantry.Instance, Units.Infantry, Modifiers.None);

            new UnitBase(ReconCar.Instance, Units.Reconnaissance, Modifiers.Wheeled);

            new UnitBase(DiveBomber.Instance, Units.FixedWingAircraft, Modifiers.None);
            new UnitBase(Fighter.Instance, Units.FixedWingAircraft | Units.AirDefence, Modifiers.None);

            new UnitBase(AntiAirHalfTrack.Instance, Units.AirDefence, Modifiers.HalfTracked);
            new UnitBase(AntiTankHalfTrack.Instance, Units.AntiTank, Modifiers.HalfTracked);
            new UnitBase(TransportHalfTrack.Instance, Units.Transportation, Modifiers.HalfTracked);

            new UnitBase(AntiAircraftArtillery.Instance, Units.Armor | Units.AirDefence, Modifiers.None);
            new UnitBase(Artillery.Instance, Units.Armor | Units.Artillery, Modifiers.None);
            new UnitBase(Tank.Instance, Units.Armor, Modifiers.None);

            new UnitBase(RocketArtillery.Instance, Units.Artillery, Modifiers.Rocket | Modifiers.Wheeled);
            new UnitBase(TransportTruck.Instance, Units.Transportation, Modifiers.Wheeled);
        }

        private static Pen Line { get; } = new Pen(Brushes.Black, 1.0);
        private static Brush Fill { get; } = Brushes.Black;
        private static Brush NoFill { get; } = Brushes.Transparent;

        private static Brush FontForeground { get; } = Brushes.AntiqueWhite;
        private static Brush FontBackground { get; } = Brushes.Black;


        public override BaseType Type { get; }
        private Units UnitSymbols { get; }
        private Modifiers UnitModifiers { get; }

        public static Geometry GetUnitClip(double maxHeight, double maxWidth)
        {
            double top = 0;
            double left = 0;
            double width = maxWidth;
            double height = maxHeight;

            var geoGroup = new GeometryGroup();
            //geoGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(maxWidth, maxHeight)));
            geoGroup.Children.Add(IdentifierFriendlyGeo(ref top, ref left, ref height, ref width));
            geoGroup.Freeze();
            return geoGroup;
        }


        public static Dictionary<UnitType, UnitBase> TYPES { get; } = new Dictionary<Game.UnitType, UnitBase>();

        private UnitBase(UnitType unitType, Units unitSymbols, Modifiers unitModifiers)
        {
            Type = unitType;
            UnitSymbols = unitSymbols;
            UnitModifiers = unitModifiers;

            TYPES.Add(unitType, this);
        }

        public Drawing GetVisualization(Identifier identifier, string hp, string concealment, double maxHeight, double maxWidth, Brush highlight)
        {
            double top = 0;
            double left = 0;
            double width = maxWidth;
            double height = maxHeight;

            var group = new DrawingGroup();
            group.Append();
            group.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Transparent, 5.0), new LineGeometry(new Point(0, 0), new Point(width, height))));

            switch (identifier)
            {
                case Identifier.Friendly:
                    group.Children.Add(IdentifierFriendly(ref top, ref left, ref height, ref width, highlight));
                    break;
                case Identifier.Hostile:
                    group.Children.Add(IdentifierEnemy(ref top, ref left, ref height, ref width, highlight));
                    break;
                default:
                    throw new NotSupportedException($"Unknown identifier {identifier}");
            }

            double bottom = top + height;
            double right = left + width;

            if(UnitSymbols.HasFlag(Units.AirDefence))
            {
                group.Children.Add(UnitsAirDefence(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.AntiTank))
            {
                group.Children.Add(UnitsAntiTank(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Armor))
            {
                group.Children.Add(UnitsArmor(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Artillery))
            {
                group.Children.Add(UnitsArtillery(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.FixedWingAircraft))
            {
                group.Children.Add(UnitsFixedWingAircraft(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Infantry))
            {
                group.Children.Add(UnitsInfantry(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Reconnaissance))
            {
                group.Children.Add(UnitsReconnaissance(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Supply))
            {
                group.Children.Add(UnitsSupply(top, left, bottom, right, height, width));
            }
            if (UnitSymbols.HasFlag(Units.Transportation))
            {
                group.Children.Add(UnitsTransportation(top, left, bottom, right, height, width));
            }

            if(UnitModifiers.HasFlag(Modifiers.HalfTracked))
            {
                group.Children.Add(ModifiersHalfTracked(top, left, bottom, right, height, width));
            }
            if (UnitModifiers.HasFlag(Modifiers.Rocket))
            {
                group.Children.Add(ModifiersRocket(top, left, bottom, right, height, width));
            }
            if (UnitModifiers.HasFlag(Modifiers.Tracked))
            {
                group.Children.Add(ModifiersTracked(top, left, bottom, right, height, width));
            }
            if (UnitModifiers.HasFlag(Modifiers.Wheeled))
            {
                group.Children.Add(ModifiersWheeled(top, left, bottom, right, height, width));
            }
            if (UnitModifiers.HasFlag(Modifiers.WheeledCrossCountry))
            {
                group.Children.Add(ModifiersWhelledCrossCountry(top, left, bottom, right, height, width));
            }

            if(hp != null)
            {
                group.Children.Add(HealthText(hp, top, left, bottom, right, height, width));
            }

            if(concealment != null)
            {
                group.Children.Add(ConcealmentText(concealment, top, left, bottom, right, height, width));
            }

            //group.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Transparent, 1.0), new LineGeometry(new Point(0, 0), new Point(width, height))));
            //group.Children.Add(RenderVisualization(height, width, background, highlight, outline));
            return group;
        }

        #region Identifier

        private static Geometry IdentifierFriendlyGeo(ref double top, ref double left, ref double height, ref double width)
        {
            var tw = (width / 6) * 4;
            var th = width * 0.30;
            left = (width - tw) / 2;
            top = (height - th) / 2;
            width = tw;
            height = th;
            return new RectangleGeometry(new Rect(left, top, width, height));
        }

        private static Drawing IdentifierFriendly(ref double top, ref double left, ref double height, ref double width, Brush highlight)
        {
            //height = 150;
            //width = 200;
            
            return new GeometryDrawing(highlight, Line, IdentifierFriendlyGeo(ref top, ref left, ref height, ref width));
        }

        private static Drawing IdentifierEnemy(ref double top, ref double left, ref double height, ref double width, Brush highlight)
        {
            height = 150;
            width = 150;
            return new GeometryDrawing(highlight, Line, new RectangleGeometry(new Rect(0, 0, width, height), 0, 0, new RotateTransform(45, width / 2, height / 2)));
        }

#endregion Identifier

        #region Units

        private Drawing UnitsInfantry(double top, double left, double bottom, double right, double height, double width)
        {
            var group = new GeometryGroup();
            group.Children.Add(new LineGeometry(new Point(left, bottom), new Point(right, top)));
            group.Children.Add(new LineGeometry(new Point(right, bottom), new Point(left, top)));

            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing UnitsReconnaissance(double top, double left, double bottom, double right, double height, double width)
        {
            return new GeometryDrawing(NoFill, Line, new LineGeometry(new Point(left, bottom), new Point(right, top)));
        }

        private Drawing UnitsAirDefence(double top, double left, double bottom, double right, double height, double width)
        {
            var path = new PathGeometry();
            var poly =
                new PathFigure(
                    new Point(left, bottom),
                        new List<PathSegment>() {
                        new ArcSegment(new Point(right, bottom), new Size(width / 2, height / 4), 0, true, SweepDirection.Clockwise, true)
                    },
                false
            );

            path.Figures.Add(poly);

            return new GeometryDrawing(NoFill, Line, path);
        }

        private Drawing UnitsAntiTank(double top, double left, double bottom, double right, double height, double width)
        {
            var group = new GeometryGroup();
            group.Children.Add(new LineGeometry(new Point(left, bottom), new Point(left + width / 2, top)));
            group.Children.Add(new LineGeometry(new Point(right, bottom), new Point(left + width / 2, top)));

            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing UnitsArmor(double top, double left, double bottom, double right, double height, double width)
        {
            var widthHalf = width * 0.6;
            var heightHalf = height  * 0.5;
            var topOffset = height - heightHalf;
            var leftOffset = width - widthHalf;

            return new GeometryDrawing(NoFill, Line, new EllipseGeometry(GetCenter(top, left, height, width), widthHalf / 2, heightHalf / 2));
        }

        private Drawing UnitsArtillery(double top, double left, double bottom, double right, double height, double width)
        {
            var radius = height * 0.3 / 2;
            return new GeometryDrawing(Fill, Line, new EllipseGeometry(GetCenter(top, left, height, width), radius, radius));
        }

        private Drawing UnitsSupply(double top, double left, double bottom, double right, double height, double width)
        {
            var lineHeight = top + height * 0.8;
            return new GeometryDrawing(NoFill, Line, new LineGeometry(new Point(left, lineHeight), new Point(right, lineHeight)));
        }

        private Drawing UnitsTransportation(double top, double left, double bottom, double right, double height, double width)
        {
            var radius = height * 0.5 / 2;
            var center = new Point(left + width / 2, top + height / 2);
            var group = new GeometryGroup();
            group.Children.Add(new LineGeometry(center, new Point(center.X - radius, center.Y)));

            var rot = 45;

            while(rot <= 360)
                group.Children.Add(new LineGeometry(center, new Point(center.X - radius, center.Y), new RotateTransform(rot+=45, center.X, center.Y)));

            group.Children.Add(new EllipseGeometry(center, radius, radius));

            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing UnitsFixedWingAircraft(double top, double left, double bottom, double right, double height, double width)
        {
            var radius = height / 3 / 2;
            var trueTop = top + radius;

            var triSize = radius * 2;
            var triOffset = Math.Sqrt(3) / 2 * triSize;

            var group = new GeometryGroup();

            var path = new PathGeometry();
            var poly =
                new PathFigure(
                    new Point(left + width / 2, top + height / 2),
                        new List<PathSegment>() {
                            new LineSegment(new Point(left + width / 2 - triOffset, top + radius * 2), true),
                            new ArcSegment(new Point(left + width / 2 - triOffset, bottom - radius * 2), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true)
                    },
                true
            );

            path.Figures.Add(poly);
            group.Children.Add(path);

            path = new PathGeometry();
            poly =
                new PathFigure(
                    new Point(left + width / 2, top + height / 2),
                        new List<PathSegment>() {
                            new LineSegment(new Point(left + width / 2 - triOffset, top + radius * 2), true),
                            new ArcSegment(new Point(left + width / 2 - triOffset, bottom - radius * 2), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true)
                    },
                true
            );

            path.Figures.Add(poly);
            path.Transform = new RotateTransform(180, left + width / 2, top + height / 2);
            group.Children.Add(path);

            return new GeometryDrawing(Fill, Line, group);
        }

        private Point GetCenter(double top, double left, double height, double width)
        {
            return new Point(left + width / 2, top + height / 2);
        }

#endregion Units

        #region Modifiers

        private Drawing ModifiersRocket(double top, double left, double bottom, double right, double height, double width)
        {
            var center = GetCenter(top, left, height, width);
            var pointTop = top + height * 0.15;
            var distY = height * 0.08;
            var distX = distY * 1.5;

            var group = new GeometryGroup();
            group.Children.Add(new LineGeometry(new Point(center.X - distX, pointTop + distY), new Point(center.X, pointTop)));
            group.Children.Add(new LineGeometry(new Point(center.X + distX, pointTop + distY), new Point(center.X, pointTop)));

            group.Children.Add(new LineGeometry(new Point(center.X - distX, pointTop + distY + distY), new Point(center.X, pointTop + distY)));
            group.Children.Add(new LineGeometry(new Point(center.X + distX, pointTop + distY + distY), new Point(center.X, pointTop + distY)));

            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing ModifiersWheeled(double top, double left, double bottom, double right, double height, double width)
        {
            double mobilityTop, mobilityLeft, mobilityBottom, mobilityRight, mobilityHeight, mobilityWidth;
            GetMobilityBox(top, left, bottom, right, height, width, out mobilityTop, out mobilityLeft, out mobilityBottom, out mobilityRight, out mobilityHeight, out mobilityWidth);


            var center = GetMobilityBoxCenter(mobilityTop, mobilityLeft, mobilityHeight, mobilityWidth);
            var radius = mobilityHeight / 2;
            var group = new GeometryGroup();
            group.Children.Add(new EllipseGeometry(new Point(mobilityLeft + radius, mobilityTop + radius), radius, radius));
            group.Children.Add(new EllipseGeometry(new Point(mobilityRight - radius, mobilityTop + radius), radius, radius));

            return new GeometryDrawing(NoFill, Line, group);

        }

        private Drawing ModifiersWhelledCrossCountry(double top, double left, double bottom, double right, double height, double width)
        {
            double mobilityTop, mobilityLeft, mobilityBottom, mobilityRight, mobilityHeight, mobilityWidth;
            GetMobilityBox(top, left, bottom, right, height, width, out mobilityTop, out mobilityLeft, out mobilityBottom, out mobilityRight, out mobilityHeight, out mobilityWidth);


            var center = GetMobilityBoxCenter(mobilityTop, mobilityLeft, mobilityHeight, mobilityWidth);
            var radius = mobilityHeight / 2;
            var group = new GeometryGroup();
            group.Children.Add(new EllipseGeometry(new Point(mobilityLeft + radius, mobilityTop + radius), radius, radius));
            group.Children.Add(new EllipseGeometry(new Point(mobilityRight - radius, mobilityTop + radius), radius, radius));
            group.Children.Add(new EllipseGeometry(center, radius, radius));

            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing ModifiersTracked(double top, double left, double bottom, double right, double height, double width)
        {
            double mobilityTop, mobilityLeft, mobilityBottom, mobilityRight, mobilityHeight, mobilityWidth;
            GetMobilityBox(top, left, bottom, right, height, width, out mobilityTop, out mobilityLeft, out mobilityBottom, out mobilityRight, out mobilityHeight, out mobilityWidth);


            var center = GetMobilityBoxCenter(mobilityTop, mobilityLeft, mobilityHeight, mobilityWidth);
            var radius = mobilityHeight / 2;
            var group = new GeometryGroup();
            var path = new PathGeometry();
            var poly =
                new PathFigure(
                    new Point(mobilityLeft + radius, mobilityTop),
                        new List<PathSegment>() {
                            new ArcSegment(new Point(mobilityLeft + radius, mobilityBottom), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true),
                            new LineSegment(new Point(mobilityRight - radius, mobilityBottom), true),
                            new ArcSegment(new Point(mobilityRight - radius, mobilityTop), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true),
                    },
                true
            );

            path.Figures.Add(poly);
            group.Children.Add(path);
            return new GeometryDrawing(NoFill, Line, group);
        }

        private Drawing ModifiersHalfTracked(double top, double left, double bottom, double right, double height, double width)
        {
            double mobilityTop, mobilityLeft, mobilityBottom, mobilityRight, mobilityHeight, mobilityWidth;
            GetMobilityBox(top, left, bottom, right, height, width, out mobilityTop, out mobilityLeft, out mobilityBottom, out mobilityRight, out mobilityHeight, out mobilityWidth);


            var center = GetMobilityBoxCenter(mobilityTop, mobilityLeft, mobilityHeight, mobilityWidth);
            var radius = mobilityHeight / 2;
            var group = new GeometryGroup();

            group.Children.Add(new EllipseGeometry(new Point(mobilityLeft + radius, mobilityTop + radius), radius, radius));

            var path = new PathGeometry();
            var poly =
                new PathFigure(
                    new Point(mobilityLeft + radius * 4, mobilityTop),
                        new List<PathSegment>() {
                            new ArcSegment(new Point(mobilityLeft + radius * 4, mobilityBottom), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true),
                            new LineSegment(new Point(mobilityRight - radius, mobilityBottom), true),
                            new ArcSegment(new Point(mobilityRight - radius, mobilityTop), new Size(radius, radius), 0, true, SweepDirection.Counterclockwise, true),
                    },
                true
            );

            path.Figures.Add(poly);
            group.Children.Add(path);
            return new GeometryDrawing(NoFill, Line, group);
        }

        private void GetMobilityBox(double top, double left, double bottom, double right, double height, double width, out double mobilityTop, out double mobilityLeft, out double mobilityBottom, out double mobilityRight, out double mobilityHeight, out double mobilityWidth)
        {
            mobilityBottom = bottom - Line.Thickness * 2;
            mobilityHeight = height * 0.15;
            mobilityWidth = width * 0.6;

            mobilityTop = mobilityBottom - mobilityHeight;
            mobilityLeft = left + (width - mobilityWidth) / 2;
            mobilityRight = mobilityLeft + mobilityWidth;
        }

        private Point GetMobilityBoxCenter(double mobilityTop, double mobilityLeft, double mobilityHeight, double mobilityWidth)
        {
            return GetCenter(mobilityTop, mobilityLeft, mobilityHeight, mobilityWidth);
        }

        #endregion Modifiers

        private Drawing HealthText(string health, double top, double left, double bottom, double right, double height, double width)
        {
            var dGroup = new DrawingGroup();
            var font = new FormattedText(health, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Lucida Console"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 10, FontForeground);

            var padding = 1;
            var fontTop = top + padding;// - font.Height;
            var fontLeft = right - padding - font.Width;
            var geo = font.BuildGeometry(new Point(fontLeft, fontTop));

            dGroup.Children.Add(new GeometryDrawing(FontBackground, new Pen(FontBackground, 0), new RectangleGeometry(new Rect(fontLeft - padding, fontTop - padding, font.Width + padding * 2, font.Height + padding * 2))));
            dGroup.Children.Add(new GeometryDrawing(FontForeground, new Pen(FontForeground, 0), geo));


            return dGroup;
        }

        private Drawing ConcealmentText(string concealment, double top, double left, double bottom, double right, double height, double width)
        {
            var dGroup = new DrawingGroup();
            var font = new FormattedText(concealment, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Lucida Console"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 10, FontForeground);

            var padding = 1;
            var fontTop = top + padding;// - font.Height;
            var fontLeft = left + padding;// - font.Width;
            var geo = font.BuildGeometry(new Point(fontLeft, fontTop));

            dGroup.Children.Add(new GeometryDrawing(FontBackground, new Pen(FontBackground, 0), new RectangleGeometry(new Rect(fontLeft - padding, fontTop - padding, font.Width + padding * 2, font.Height + padding * 2))));
            dGroup.Children.Add(new GeometryDrawing(FontForeground, new Pen(FontForeground, 0), geo));


            return dGroup;
        }

        [Flags]
        public enum Units {
            None = 0,
            Infantry = 1,
            Reconnaissance = 2,
            AirDefence = 4,
            AntiTank = 8,
            Armor = 16,
            Artillery = 32,
            Supply = 128,
            Transportation = 256,
            FixedWingAircraft = 512
        }

        [Flags]
        public enum Modifiers {
            None = 0,
            Rocket = 1,
            Wheeled = 2,
            WheeledCrossCountry = 4,
            Tracked = 8,
            HalfTracked = 16
        }

        public enum Identifier { Friendly, Hostile }
    }
}
