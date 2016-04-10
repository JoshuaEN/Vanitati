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

        public bool Highlight
        {
            set
            {
                if(value == true)
                {
                    OverlayHighlight.Visibility = Visibility.Visible;
                }
                else
                {
                    OverlayHighlight.Visibility = Visibility.Collapsed;
                }
            }
        }

        private BitmapImage TerrainBitmap { get; set; }
        private BitmapImage UnitBitmap { get; set; }

        private ImageDrawing TerrainImage { get; set; }
        private ImageDrawing UnitImage { get; set; }

        private DrawingGroup Collection { get; } = new DrawingGroup();
        private Polygon OverlayHighlight { get; }

        public TileView(BattleView view, Location location)
        {
            InitializeComponent();
            imageContainer = new OpaqueClickableImage(this);

            View = view;
            Location = location;

            UpdateTerrain();
            UpdateUnit();
            UpdateScale();

            Collection.Children.Add(TerrainImage);
            Collection.Children.Add(UnitImage);
            

            imageContainer.Source = new DrawingImage(Collection); // new DrawingImage(new ImageDrawing(TerrainBitmap, new Rect(0, 0, Width, Height)));


            //TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_plain.png"), UriKind.Relative));
            contentGrid.Children.Add(imageContainer);
            contentGrid.Children.Add(new Label() { Content = location, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, IsHitTestVisible = false });

            var poly = new Polygon();
            poly.Points.Add(new Point(0, Height / 2));
            poly.Points.Add(new Point(Width * 0.25, Height));
            poly.Points.Add(new Point(Width * 0.75, Height));
            poly.Points.Add(new Point(Width, Height / 2));
            poly.Points.Add(new Point(Width * 0.75, 0));
            poly.Points.Add(new Point(Width * 0.25, 0));
            poly.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            poly.IsHitTestVisible = false;
            OverlayHighlight = poly;

            OverlayHighlight.Visibility = Visibility.Collapsed;
            contentGrid.Children.Add(OverlayHighlight);

            imageContainer.Stretch = Stretch.None;
            RenderOptions.SetBitmapScalingMode(imageContainer, BitmapScalingMode.Fant);
            imageContainer.MouseEnter += ImageContainer_MouseEnter;
            imageContainer.MouseLeave += ImageContainer_MouseLeave;
            imageContainer.MouseUp += ImageContainer_MouseUp;
            imageContainer.MouseDown += ImageContainer_MouseDown;

            UpdateScale();
            UpdatePosition();
        }

        private void ImageContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var handler = TileViewMouseDown;
            if(null != handler)
            {
                handler(this, new TileViewEventArgs(this, e));
            }
        }

        private void ImageContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var handler = TileViewMouseUp;
            if (null != handler)
            {
                handler(this, new TileViewEventArgs(this, e));
            }
        }

        public event EventHandler<TileViewEventArgs> TileViewMouseDown;
        public event EventHandler<TileViewEventArgs> TileViewMouseUp;

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

        private void ImageContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            //TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_plain.png"), UriKind.Relative));
            //UpdatePosition();
        }

        private void ImageContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            //TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_road.png"), UriKind.Relative));
           //UpdatePosition();
        }

        public void UpdateTerrain()
        {
            var terrain = View.State.GetTerrain(Location);
            TerrainBitmap = Resource.SPRITES[terrain.TerrainType.Key];
            UpdateTerrainImage();
        }

        public void UpdateUnit()
        {
            var unit = View.State.GetUnit(Location);

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

        private void UpdateTerrainImage()
        {
            TerrainImage = new ImageDrawing(TerrainBitmap, new Rect(0, 0, Width, Height));

            if(Collection.Children.Count == 2)
                Collection.Children[0] = TerrainImage;

        }

        private void UpdateUnitImage()
        {
            UnitImage = new ImageDrawing(UnitBitmap, new Rect(0, 0, UnitBitmap.PixelWidth * View.Scale, UnitBitmap.PixelHeight * View.Scale));
            if (Collection.Children.Count == 2)
                Collection.Children[1] = UnitImage;

        }

        public void UpdateScale()
        {
            Height = TerrainBitmap.PixelHeight * View.Scale;
            Width = TerrainBitmap.PixelWidth * View.Scale;

            UpdateTerrainImage();
            UpdateUnitImage();
           
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var X = Location.X * Width * 0.75 + -View.XOffset;
            var Y = Location.Y * Height * 1 + -View.YOffset;

            if (Location.X % 2 == 0)
            {
                Canvas.SetLeft(this, X);
                Canvas.SetTop(this, Y);
            }
            else
            {
                Canvas.SetLeft(this, X);// + Width * 0.75);
                Canvas.SetTop(this, Y + Height * 0.5);
            }

        }

        public static TileView GetTileView(IInputElement elm)
        {
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

        public class TileViewEventArgs
        {
            public TileView TileView { get; }
            public MouseButtonEventArgs Args { get; }

            public TileViewEventArgs(TileView tileView, MouseButtonEventArgs args)
            {
                TileView = tileView;
                Args = args;
            }
        }
    }
}
