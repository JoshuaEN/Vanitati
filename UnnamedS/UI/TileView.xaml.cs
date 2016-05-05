using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for TileView.xaml
    /// </summary>
    public partial class TileView : UserControl
    {
        private OpaqueClickableImage imageContainer;
        private BattleView View { get; }

        private Location _location;
        public Location Location
        {
            get { return _location; }
            set
            {
                Contract.Requires<ArgumentNullException>(null != value);
                _location = value;
                UpdatePosition();
            }
        }

        private HighlightMode _highlight = HighlightMode.None;
        public HighlightMode Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                UpdateHighlights();
            }
        }

        public enum HighlightMode { None, Ally, Enemy, Neutral, Selected }

        private class BrushSet
        {
            public Brush OverlayBrush { get; set; } = new SolidColorBrush(Colors.Transparent);
            public Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Transparent);
        }

        private Dictionary<HighlightMode, BrushSet> HighlightBrushSets = new Dictionary<HighlightMode, BrushSet>()
        {
            { HighlightMode.None, new BrushSet() },
            { HighlightMode.Ally, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)) } },
            { HighlightMode.Enemy, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)) } },
            { HighlightMode.Neutral, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)) } },
            { HighlightMode.Selected, new BrushSet() {OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200)) } }
        };

        private BitmapImage TerrainBitmap { get; set; }
        private BitmapImage UnitBitmap { get; set; }

        private ImageDrawing TerrainImage { get; set; }
        private ImageDrawing UnitImage { get; set; }

        private DrawingGroup Collection { get; } = new DrawingGroup();


        private Polygon OverlayHighlight { get; }
        private Polyline BorderHighlight { get; }
        private Polygon TerrainPoly { get; }
        private Polyline GridPolyline { get; }
        private TextBlock TerrainLabel { get; } = new TextBlock();
        private Border TerrainBorder { get; }
        private TextBlock UnitLabel { get; } = new TextBlock();
        private Border UnitBorder { get; }

        private TextBlock XYTopLeftLabel { get; } = new TextBlock();
        private TextBlock XYTopRightLabel { get; } = new TextBlock();

        private static readonly bool TEXT_MODE = true;

        public TileView(BattleView view, Location location)
        {
            InitializeComponent();
            imageContainer = new OpaqueClickableImage(this);

            View = view;
            Location = location;

            if (TEXT_MODE)
            {
                UpdateScale();
                TerrainPoly = GetHexPoly();
            }
            else
            { 
                UpdateTerrain();
                UpdateUnit();
                UpdateScale();
            }

            if (TEXT_MODE)
            {
                TerrainPoly.IsHitTestVisible = true;
                TerrainPoly.SnapsToDevicePixels = false;
                TerrainPoly.MouseEnter += ImageContainer_MouseEnter;
                TerrainPoly.MouseLeave += ImageContainer_MouseLeave;
                TerrainPoly.MouseUp += ImageContainer_MouseUp;
                TerrainPoly.MouseDown += ImageContainer_MouseDown;
                RenderOptions.SetEdgeMode(TerrainPoly, EdgeMode.Aliased);

                contentGrid.Children.Add(TerrainPoly);

                GridPolyline = GetHexPolyline();
                GridPolyline.IsHitTestVisible = false;
                GridPolyline.Stroke = Brushes.Beige;
                GridPolyline.StrokeLineJoin = PenLineJoin.Miter;
                GridPolyline.StrokeThickness = 1;
                contentGrid.Children.Add(GridPolyline);

                TerrainLabel.VerticalAlignment = VerticalAlignment.Top;
                TerrainLabel.HorizontalAlignment = HorizontalAlignment.Center;
                TerrainLabel.IsHitTestVisible = false;
                TerrainBorder = GetGenericBorder(TerrainLabel);
                TerrainBorder.BorderThickness = new Thickness(0, 0, 0, 4);
                contentGrid.Children.Add(TerrainBorder);

                UnitLabel.VerticalAlignment = VerticalAlignment.Center;
                UnitLabel.HorizontalAlignment = HorizontalAlignment.Center;
                UnitLabel.IsHitTestVisible = false;
                UnitLabel.FontSize = 14;
                UnitBorder = GetGenericBorder(UnitLabel);
                UnitBorder.BorderThickness = new Thickness(0, 0, 0, 4);
                contentGrid.Children.Add(UnitBorder);

                UpdateTerrain();
                UpdateUnit();
                UpdateScale();
            }
            else
            {
                Collection.Children.Add(TerrainImage);
                Collection.Children.Add(UnitImage);

                imageContainer.Source = new DrawingImage(Collection);
                imageContainer.Stretch = Stretch.None;
                RenderOptions.SetBitmapScalingMode(imageContainer, BitmapScalingMode.Fant);
                imageContainer.MouseEnter += ImageContainer_MouseEnter;
                imageContainer.MouseLeave += ImageContainer_MouseLeave;
                imageContainer.MouseUp += ImageContainer_MouseUp;
                imageContainer.MouseDown += ImageContainer_MouseDown;
                RenderOptions.SetEdgeMode(imageContainer, EdgeMode.Aliased);

                contentGrid.Children.Add(imageContainer);

                GridPolyline = GetHexPolyline();
                GridPolyline.IsHitTestVisible = false;
                GridPolyline.Stroke = Brushes.Beige;
                GridPolyline.StrokeLineJoin = PenLineJoin.Miter;
                GridPolyline.StrokeThickness = 1;
                GridPolyline.SnapsToDevicePixels = false;
                contentGrid.Children.Add(GridPolyline);
            }

            //contentGrid.Children.Add(new Label() { Content = location, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Center, IsHitTestVisible = false });

            XYTopLeftLabel.VerticalAlignment = VerticalAlignment.Bottom;
            XYTopLeftLabel.HorizontalAlignment = HorizontalAlignment.Center;
            XYTopLeftLabel.IsHitTestVisible = false;
            XYTopLeftLabel.Text = Location.ToString();
            contentGrid.Children.Add(XYTopLeftLabel);

            XYTopRightLabel.VerticalAlignment = VerticalAlignment.Bottom;
            XYTopRightLabel.HorizontalAlignment = HorizontalAlignment.Right;
            XYTopRightLabel.IsHitTestVisible = false;
            contentGrid.Children.Add(XYTopRightLabel);

            OverlayHighlight = GetHexPoly();
            OverlayHighlight.IsHitTestVisible = false;
            contentGrid.Children.Add(OverlayHighlight);

            BorderHighlight = GetHexPolyline();
            BorderHighlight.IsHitTestVisible = false;
            BorderHighlight.StrokeThickness = 4;
            contentGrid.Children.Add(BorderHighlight);

            UpdateHighlights();

            UpdateScale();
            UpdatePosition();
        }

        private Polygon GetHexPoly()
        {
            var poly = new Polygon();

            foreach (var point in GetHexPoints())
                poly.Points.Add(point);

            return poly;
        }

        private Polyline GetHexPolyline()
        {
            var polyline = new Polyline();

            foreach (var point in GetHexPoints())
                polyline.Points.Add(point);

            // Close the line.
            polyline.Points.Add(GetHexPoints()[0]);

            return polyline;
        }

        private Border GetGenericBorder(FrameworkElement elm)
        {
            return new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), Child = elm, Width = double.NaN, Height = double.NaN, Padding = new Thickness(2), Margin = new Thickness(0), HorizontalAlignment = elm.HorizontalAlignment, VerticalAlignment = elm.VerticalAlignment, IsHitTestVisible = false, Background = Brushes.AntiqueWhite};
        }

        private Point[] GetHexPoints()
        {
            return new Point[]
            {
                new Point(0, Height / 2),
                new Point(Width * 0.25, Height),
                new Point(Width * 0.75, Height),
                new Point(Width, Height / 2),
                new Point(Width * 0.75, 0),
                new Point(Width * 0.25, 0)
            };
        }

        #region Event Handlers

        private void ImageContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var handler = TileViewMouseDown;
            if(null != handler)
            {
                handler(this, new TileViewMouseButtonEventArgs(this, e));
            }
        }

        private void ImageContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var handler = TileViewMouseUp;
            if (null != handler)
            {
                handler(this, new TileViewMouseButtonEventArgs(this, e));
            }
        }

        private void ImageContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            TileViewMouseLeave?.Invoke(this, new TileViewMouseEventArgs(this, e));
        }

        private void ImageContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            TileViewMouseEnter?.Invoke(this, new TileViewMouseEventArgs(this, e));
        }

        public event EventHandler<TileViewMouseButtonEventArgs> TileViewMouseDown;
        public event EventHandler<TileViewMouseButtonEventArgs> TileViewMouseUp;
        public event EventHandler<TileViewMouseEventArgs> TileViewMouseEnter;
        public event EventHandler<TileViewMouseEventArgs> TileViewMouseLeave;

        public new event MouseEventHandler MouseEnter
        {
            add { imageContainer.MouseEnter += value; }
            remove { imageContainer.MouseEnter -= value; }
        }

        public new event MouseEventHandler MouseLeave
        {
            add { imageContainer.MouseLeave += value; }
            remove { imageContainer.MouseLeave -= value; }
        }
        
        public new event MouseButtonEventHandler MouseDown
        {
            add { imageContainer.MouseDown += value; }
            remove { imageContainer.MouseDown -= value; }
        }

        public new event MouseButtonEventHandler MouseUp
        {
            add { imageContainer.MouseUp += value; }
            remove { imageContainer.MouseUp -= value; }
        }

        public new event MouseEventHandler MouseMove
        {
            add { imageContainer.MouseMove += value; }
            remove { imageContainer.MouseMove -= value; }
        }

        #endregion

        private void UpdateHighlights()
        {
            BrushSet set = HighlightBrushSets[Highlight];

            OverlayHighlight.Fill = set.OverlayBrush;
            BorderHighlight.Stroke = set.BorderBrush;
        }

        public void UpdateTerrain()
        {
            var terrain = View.State.GetTerrain(Location);

            if (TEXT_MODE)
            {
                TerrainPoly.Fill = Resource.BRUSHES[terrain.TerrainType.Key];
                TerrainPoly.InvalidateVisual();
                TerrainLabel.Text = Globals.GetResource(terrain.TerrainType.Key);

                var bytes = new byte[3];

                if (terrain.IsOwned)
                {
                    new Random(terrain.CommanderID).NextBytes(bytes);
                    TerrainBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));
                }
                else
                {
                    TerrainBorder.BorderBrush = Brushes.Transparent;
                }
            }
            else
            {
                TerrainBitmap = Resource.SPRITES[terrain.TerrainType.Key];
                UpdateTerrainImage();
            }
        }

        public void UpdateUnit()
        {
            var unit = View.State.GetUnit(Location);

            if (TEXT_MODE)
            {
                if(null != unit)
                {
                    UnitLabel.Text = Globals.GetResource(unit.UnitType.Key);
                    
                    var bytes = new byte[3];
                    new Random(unit.CommanderID).NextBytes(bytes);
                    UnitBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, bytes[0], bytes[1], bytes[2]));
                    UnitBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    UnitLabel.Text = "";
                    UnitBorder.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (null != unit)
                {
                    UnitBitmap = Resource.SPRITES[unit.UnitType.Key];
                }
                else
                {
                    UnitBitmap = Resource.BITMAP_NO_UNIT;
                }

                UpdateUnitImage();
            }
        }

        private void UpdateTerrainImage()
        {
            if (TEXT_MODE)
                return;

            TerrainImage = new ImageDrawing(TerrainBitmap, new Rect(0, 0, Width, Height));

            if(Collection.Children.Count == 2)
                Collection.Children[0] = TerrainImage;

        }

        private void UpdateUnitImage()
        {
            if (TEXT_MODE)
                return;

            UnitImage = new ImageDrawing(UnitBitmap, new Rect(0, 0, UnitBitmap.PixelWidth * View.Scale, UnitBitmap.PixelHeight * View.Scale));
            if (Collection.Children.Count == 2)
                Collection.Children[1] = UnitImage;
        }

        public void UpdateScale()
        {
            if (true)
            {
                var x1 = Math.Floor(512 * View.Scale);
                Width = x1 * 2; // Resource.BITMAP_HIT_REFERENCE.PixelHeight * View.Scale;
                Height = Math.Floor(Math.Sqrt(3) * x1); //Resource.BITMAP_HIT_REFERENCE.PixelWidth * View.Scale;
            }
            else
            {
                Height = Resource.BITMAP_HIT_REFERENCE.PixelHeight * View.Scale;
                Width = Resource.BITMAP_HIT_REFERENCE.PixelWidth * View.Scale;
            }

            UpdateTerrainImage();
            UpdateUnitImage();
           
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var offset_x = Math.Floor(Location.X * 0.0);
            var offset_y = Math.Floor(Location.Y * 0.0);
            var X = (Location.X * Math.Round(Width * 0.75)  + offset_x ) - Math.Round(View.XOffset);
            var Y = Location.Y * Height * 1 + -View.YOffset;

            if (Location.X % 2 != 0)
            {
                //X -= 1;
                Y = Y + Height * 0.5;
            }

            //X = Math.Floor(X);
            Y = Math.Floor(Y);

            Canvas.SetLeft(this, X);
            Canvas.SetTop(this, Y);

            //if (Location.X % 2 != 0)
            //    XYTopLeftLabel.Content = $"{X + (Width / 4)},{Y}\n{X + (Width * 0.75)},{Y + Height}";
            //else
            //    XYTopLeftLabel.Content = $"{X},{Y}\n{X + Width},{Y + Height}";

        }

        public static TileView GetTileView(IInputElement elm)
        {
            var control = (elm as FrameworkElement);

            while(control != null)
            {
                var tile = (control as TileView);

                if (tile != null)
                    return tile;

                control = (FrameworkElement)control.Parent;
            }

            return null;

            var opaqueImg = (elm as OpaqueClickableImage);

            if (opaqueImg == null)
                return null;

            return opaqueImg.TileView;
        }

        // Adapted from http://stackoverflow.com/a/7955547
        private class OpaqueClickableImage : Image
        {
            public TileView TileView { get; }

            public OpaqueClickableImage(TileView tileView)
            {
                TileView = tileView;
            }

            protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
                var x = (int)(hitTestParameters.HitPoint.X / ActualWidth * Resource.BITMAP_HIT_REFERENCE.PixelWidth);
                var y = (int)(hitTestParameters.HitPoint.Y / ActualHeight * Resource.BITMAP_HIT_REFERENCE.PixelHeight);

                if (x == Resource.BITMAP_HIT_REFERENCE.PixelWidth)
                {
                    x--;
                }

                if (y == Resource.BITMAP_HIT_REFERENCE.PixelHeight)
                {
                    y--;
                }

                var pixels = new byte[4];
                Resource.BITMAP_HIT_REFERENCE.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);

                if (pixels[3] < 1)
                {
                    return null;
                }

                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
        }

        public class TileViewMouseButtonEventArgs
        {
            public TileView TileView { get; }
            public MouseButtonEventArgs Args { get; }

            public TileViewMouseButtonEventArgs(TileView tileView, MouseButtonEventArgs args)
            {
                TileView = tileView;
                Args = args;
            }
        }

        public class TileViewMouseEventArgs
        {
            public TileView TileView { get; }
            public MouseEventArgs Args { get; }

            public TileViewMouseEventArgs(TileView tileView, MouseEventArgs args)
            {
                TileView = tileView;
                Args = args;
            }
        }
    }
}
