using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _location = value;
                UpdatePosition();
            }
        }

        private BitmapImage TerrainBitmap { get; set; }
        private BitmapImage UnitBitmap { get; set; }

        private ImageDrawing TerrainImage { get; set; }
        private ImageDrawing UnitImage { get; set; }

        public TileView(BattleView view, Location location)
        {
            InitializeComponent();

            var collection = new DrawingGroup();

            collection.Children.Add(TerrainImage);
            collection.Children.Add(UnitImage);

            imageContainer = new OpaqueClickableImage();
            imageContainer.Source = new DrawingImage(collection); // new DrawingImage(new ImageDrawing(TerrainBitmap, new Rect(0, 0, Width, Height)));


            //TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_plain.png"), UriKind.Relative));
            Content = imageContainer;

            imageContainer.Stretch = Stretch.None;
            RenderOptions.SetBitmapScalingMode(imageContainer, BitmapScalingMode.Fant);
            imageContainer.MouseEnter += ImageContainer_MouseEnter;
            imageContainer.MouseLeave += ImageContainer_MouseLeave;

            View = view;
            Location = location;

            UpdateTerrain();
            UpdateUnit();
            UpdateScale();
            UpdatePosition();
        }

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
            TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_plain.png"), UriKind.Relative));
            UpdatePosition();
        }

        private void ImageContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            TerrainBitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_road.png"), UriKind.Relative));
            UpdatePosition();
        }

        private const double X_OFFSET = 1.5;
        private const double GRID_OFFSET = 10;

        public void UpdateTerrain()
        {
            var terrain = View.GameState.GetTerrain(Location);

            var attr = terrain.GetAttribute(TerrainType.TYPE);

            TerrainBitmap = Resource.SPRITES[attr.GetValue<TerrainType>().Key];
        }

        public void UpdateUnit()
        {
            var unit = View.GameState.GetUnit(Location);

            var attr = unit.GetAttribute(UnitType.TYPE);

            TerrainBitmap = Resource.SPRITES[attr.GetValue<UnitType>().Key];
        }

        public void UpdateScale()
        {
            Height = TerrainBitmap.PixelHeight * View.Scale;
            Width = TerrainBitmap.PixelWidth * View.Scale;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var X = Location.X * Width * X_OFFSET;
            var Y = Location.Y * Height * 0.5;

            if (Location.Y % 2 == 0)
            {
                Canvas.SetLeft(this, X);
                Canvas.SetTop(this, Y);
            }
            else
            {
                Canvas.SetLeft(this, X + Width * 0.75);
                Canvas.SetTop(this, Y);
            }

        }

        // Adapted from http://stackoverflow.com/a/7955547
        private class OpaqueClickableImage : Image
        {
            private static readonly BitmapImage BITMAP_HIT_REFERENCE =
                new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_hit_reference.png"), UriKind.Relative));

            protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
                var x = (int)(hitTestParameters.HitPoint.X / ActualWidth * BITMAP_HIT_REFERENCE.PixelWidth);
                var y = (int)(hitTestParameters.HitPoint.Y / ActualHeight * BITMAP_HIT_REFERENCE.PixelHeight);

                if (x == BITMAP_HIT_REFERENCE.PixelWidth)
                {
                    x--;
                }

                if (y == BITMAP_HIT_REFERENCE.PixelHeight)
                {
                    y--;
                }

                var pixels = new byte[4];
                BITMAP_HIT_REFERENCE.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);

                if (pixels[3] < 1)
                {
                    return null;
                }

                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
        }
    }
}
