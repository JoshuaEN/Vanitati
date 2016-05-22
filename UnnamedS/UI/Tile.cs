using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.UI.TileUI;

namespace UnnamedStrategyGame.UI
{
    public class Tile
    {
        private static readonly BitmapTileCache TileCache = new BitmapTileCache();

        public BattleViewV2 View { get; set; }

        public Geometry Clip { get; private set; }
        public Geometry UnitClip { get; private set; }

        public double Scale { get; set; }

        private IReadOnlyBattleGameState State { get; set; }

        public Location Location { get; }

        public const int BASE_HEX_SIZE = 512;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public int Top { get; private set; }
        public int Left { get; private set; }

        private Drawing TerrainDrawing { get; set; }
        private Drawing UnitDrawing { get; set; }

        private Drawing GridDrawing { get; set; }
        private Drawing OverlayDrawing { get; set; }

        private Drawing BlankDrawing { get; set; }

        private int CurrentUnitHealth { get; set; }
        private UnitType CurrentUnitType { get; set; }
        private int CurrentUnitConcealment { get; set; }
        private int CurrentUnitCommanderID { get; set; }
        private bool CurrentUnitHasScaleChanged { get; set; }

        private int CurrentTerrainCommanderID { get; set; }
        private bool CurrentTerrainIsOwned { get; set; }
        private TerrainType CurrentTerrainType { get; set; }
        private bool CurrentTerrainHasScaleChanged { get; set; }

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

        public enum HighlightMode { None, Ally, Enemy, Neutral, Selected, SelectionAvailable, SelectionUnavailable }

        private Dictionary<HighlightMode, BrushSet> HighlightBrushSets = new Dictionary<HighlightMode, BrushSet>()
        {
            { HighlightMode.None, new BrushSet() },
            { HighlightMode.Ally, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)) } },
            { HighlightMode.Enemy, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)) } },
            { HighlightMode.Neutral, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)) } },
            { HighlightMode.Selected, new BrushSet() {OverlayBrush = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200)) } },
            { HighlightMode.SelectionAvailable, new BrushSet() { OverlayBrush = Brushes.Transparent } },
            { HighlightMode.SelectionUnavailable, new BrushSet() { OverlayBrush = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)) } }
        };

        private bool HasUnit { get; set; }
        private bool HasHighlight { get; set; }
        private Brush HighlightBrush { get; set; } 

        private Rectangle DirectCanvas { get; } = new Rectangle() { Fill = Brushes.Transparent };
        private Rectangle NeutralOverlayCanvas { get; } = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)) };
        private Rectangle EnemyOverlayCanvas { get; } = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)) };
        private Rectangle AllyOverlayCanvas { get; } = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)) };
        private Rectangle SelectionUnavailableCanvas { get; } = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)) };
        private Rectangle UnitExpendedCanvas { get; } = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)) };

        public class CanvasSet
        {
            public Canvas View { get; }
            public Canvas OverlayAlly { get; }
            public Canvas OverlayEnemy { get; }
            public Canvas OverlayNeutral { get; }
            public Canvas SelectionUnavailable { get; }
            public Canvas UnitExpended { get; }

            public CanvasSet(Canvas view, Canvas overlayAlly = null, Canvas overlayEnemy = null, Canvas overlayNeutral = null, Canvas selectionUnavailable = null, Canvas unitExpended = null)
            {
                Contract.Requires<ArgumentNullException>(null != view);
                View = view;
                OverlayAlly = overlayAlly;
                OverlayEnemy = overlayEnemy;
                OverlayNeutral = overlayNeutral;
                SelectionUnavailable = selectionUnavailable;
                UnitExpended = unitExpended;
            }
        }

        public Tile(CanvasSet canvasSet, IReadOnlyBattleGameState state, Location location, double scale)
        {
            Contract.Requires<ArgumentNullException>(null != canvasSet);
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != location);

            BlankDrawing = new GeometryDrawing(null, new Pen(null, 0), new RectangleGeometry(new Rect(0, 0, 0, 0)));
            BlankDrawing.Freeze();

            State = state;
            Location = location;
            Scale = scale;

            DirectCanvas.MouseDown += TerrainPoly_MouseDown;
            DirectCanvas.MouseUp += TerrainPoly_MouseUp;
            DirectCanvas.MouseEnter += TerrainPoly_MouseEnter;
            DirectCanvas.MouseLeave += TerrainPoly_MouseLeave;

            //DirectCanvas.CacheMode = new BitmapCache();
            canvasSet.View.Children.Add( DirectCanvas);
            canvasSet.OverlayAlly?.Children.Add(AllyOverlayCanvas);
            canvasSet.OverlayEnemy?.Children.Add(EnemyOverlayCanvas);
            canvasSet.OverlayNeutral?.Children.Add(NeutralOverlayCanvas);
            canvasSet.SelectionUnavailable?.Children.Add(SelectionUnavailableCanvas);
            canvasSet.UnitExpended?.Children.Add(UnitExpendedCanvas);

            DirectCanvas.ClipToBounds = false;
            //DirectCanvas.CacheMode = new BitmapCache() { EnableClearType = false, SnapsToDevicePixels = false };
            

            UpdateScale();

            UpdateOverlayDrawing();
            UpdateGridDrawing();
            UpdateTerrain();
            UpdateUnit();
        }

        #region Events

        public event EventHandler<TileMouseEventArgs> MouseEnter;
        public event EventHandler<TileMouseEventArgs> MouseLeave;
        public event EventHandler<TileMouseButtonEventArgs> MouseUp;
        public event EventHandler<TileMouseButtonEventArgs> MouseDown;

        private void TerrainPoly_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave?.Invoke(this, new TileMouseEventArgs(Location, e));
        }

        private void TerrainPoly_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter?.Invoke(this, new TileMouseEventArgs(Location, e));
        }

        private void TerrainPoly_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(this, new TileMouseButtonEventArgs(Location, e));
        }

        private void TerrainPoly_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, new TileMouseButtonEventArgs(Location, e));
        }

        #endregion

        #region Visual Updates

        public void UpdateTerrain()
        {
            UpdateDrawings2();
            return;
            var terrain = State.GetTerrain(Location);

            if (terrain.TerrainType == CurrentTerrainType && terrain.CommanderID == CurrentTerrainCommanderID && terrain.IsOwned == CurrentTerrainIsOwned && CurrentTerrainHasScaleChanged == false)
                return;

            CurrentTerrainHasScaleChanged = false;
            CurrentTerrainType = terrain.TerrainType;
            CurrentTerrainIsOwned = terrain.IsOwned;
            CurrentTerrainCommanderID = terrain.CommanderID;

            Brush brush;
            if (terrain.IsOwned)
            {
                brush = Globals.GetCommanderColor(terrain.CommanderID);
            }
            else
            {
                if (terrain.TerrainType.CanCapture)
                    brush = Resource.NPC_BRUSH;
                else
                    brush = Resource.CANNOT_BE_CAPTURED_BRUSH;

            }

            TerrainBase baseUI;

            if (TerrainBase.TYPES.TryGetValue(terrain.TerrainType, out baseUI))
            {
                TerrainDrawing = baseUI.GetVisualization(terrain, Height, Width, Brushes.Gray, brush, new Pen(Brushes.Black, 1));
            }
            else
            {
                TerrainDrawing = null;
            }
            UpdateDrawings();
        }

        public void UpdateUnit()
        {
            UpdateDrawings2();
            return;
            var unit = State.GetUnit(Location);
            var terrain = State.GetTerrain(Location);
            var concealment = unit?.GetEffectiveConcealment(State, terrain);

            if (unit?.UnitType == CurrentUnitType && CurrentUnitHasScaleChanged == false && (unit == null || (unit.Health == CurrentUnitHealth && concealment.Value == CurrentUnitConcealment && unit.CommanderID == CurrentUnitCommanderID)))
                return;

            CurrentUnitHasScaleChanged = false;
            CurrentUnitType = unit?.UnitType;

            if (unit != null)
            {
                CurrentUnitHealth = unit.Health;
                CurrentUnitConcealment = concealment.Value;
                CurrentUnitCommanderID = unit.CommanderID;

                UnitBase baseUI;
                if (UnitBase.TYPES.TryGetValue(unit.UnitType, out baseUI))
                {
                    UnitDrawing = baseUI.GetVisualization(UnitBase.Identifier.Friendly, (unit.Health != unit.UnitType.MaxHealth ? unit.Health.ToString() : null), unit.GetEffectiveConcealment(State, State.GetTerrain(Location)).ToString(), Height, Width, Globals.GetCommanderColor(unit.CommanderID));
                }
                else
                {
                    UnitDrawing = null;
                    
                }

                HasUnit = true;
            }
            else
            {
                HasUnit = false;
            }

            UpdateDrawings();
        }

        public void UpdateScale()
        {

            var x1 = 512 * Scale;
            Width = (int)Math.Round(x1 * 2);
            Height = (int)Math.Round(Math.Sqrt(3) * x1);

            DirectCanvas.Height = Height;
            DirectCanvas.Width = Width;

            AllyOverlayCanvas.Height = Height;
            AllyOverlayCanvas.Width = Width;
            EnemyOverlayCanvas.Height = Height;
            EnemyOverlayCanvas.Width = Width;
            NeutralOverlayCanvas.Height = Height;
            NeutralOverlayCanvas.Width = Width;
            SelectionUnavailableCanvas.Height = Height;
            SelectionUnavailableCanvas.Width = Width;
            UnitExpendedCanvas.Height = Height;
            UnitExpendedCanvas.Width = Width;

            var points = GetHexPoints(Height, Width);
            DirectCanvas.Clip = new PathGeometry(new List<PathFigure>() { new PathFigure(points[0], new List<PathSegment>(1) { new PolyLineSegment(points, false) }, true) });
            AllyOverlayCanvas.Clip = DirectCanvas.Clip;
            EnemyOverlayCanvas.Clip = DirectCanvas.Clip;
            NeutralOverlayCanvas.Clip = DirectCanvas.Clip;
            SelectionUnavailableCanvas.Clip = DirectCanvas.Clip;
            UnitExpendedCanvas.Clip = DirectCanvas.Clip;
            UpdateLocation();

            var newPoints = new List<Point>();

            foreach(var point in points)
            {
                newPoints.Add(new Point(Left + point.X, Top + point.Y));
            }
            Clip = new PathGeometry(new List<PathFigure>() { new PathFigure(newPoints[0], new List<PathSegment>(1) { new PolyLineSegment(newPoints, false) }, true) });
            var clipGeo = UnitBase.GetUnitClip(Height, Width);
            UnitClip = new RectangleGeometry(new Rect(clipGeo.Bounds.Left + Left + 1.0, clipGeo.Bounds.Top + Top + 1.0, clipGeo.Bounds.Width - 2.0, clipGeo.Bounds.Height - 2.0));

            CurrentUnitHasScaleChanged = true;
            CurrentTerrainHasScaleChanged = true;

        }

        public void UpdateLocation()
        {
            var left = 0;
            for(var x = 0; x < Location.X; x++)
            {
                left += (int)Math.Floor((Width * 0.75));
            }

            var top = 0;
            for (var y = 0; y < Location.Y; y++)
            {
                top += Height;
            }

            if (Location.X % 2 != 0)
            {
                top += (int)Math.Floor(Height * 0.5);
            }

            Top = top;
            Left = left;

            Canvas.SetTop(DirectCanvas, Top);
            Canvas.SetLeft(DirectCanvas, Left);

            Canvas.SetTop(AllyOverlayCanvas, Top);
            Canvas.SetLeft(AllyOverlayCanvas, Left);
            Canvas.SetTop(EnemyOverlayCanvas, Top);
            Canvas.SetLeft(EnemyOverlayCanvas, Left);
            Canvas.SetTop(NeutralOverlayCanvas, Top);
            Canvas.SetLeft(NeutralOverlayCanvas, Left);
            Canvas.SetTop(SelectionUnavailableCanvas, Top);
            Canvas.SetLeft(SelectionUnavailableCanvas, Left);
            Canvas.SetTop(UnitExpendedCanvas, Top);
            Canvas.SetLeft(UnitExpendedCanvas, Left);
        }

        public void UpdateGridDrawing()
        {
            UpdateDrawings2();
            return;
            var path = new PathGeometry();

            var segments = new List<PathSegment>();

            var height = Height;
            var width = Width;

            var stroke = 4.0;
            var halfStroke = 2.0;

            var points = new Point[]
            {
                new Point(0 - halfStroke, height / 2),
                new Point(width * 0.25 - halfStroke, height + halfStroke),
                new Point(width * 0.75 + halfStroke, height + halfStroke),
                new Point(width + halfStroke, height / 2),
                new Point(width * 0.75 + halfStroke, 0 - halfStroke),
                new Point(width * 0.25 - halfStroke, 0 - halfStroke)
            };

            for (var i = 1; i < points.Length; i++)
            {
                var seg = new LineSegment(points[i], true);
                seg.Freeze();
                segments.Add(seg);
            }

            var poly =
                new PathFigure(
                    points[0],
                    segments,
                    true
                );

            poly.Freeze();
            path.Figures.Add(poly);

            //var fmTxt = new FormattedText(Location.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 24, Brushes.Black);
            //path.AddGeometry(fmTxt.BuildGeometry(new Point(width / 2, height / 2)));

            path.Freeze();

            GridDrawing = new GeometryDrawing(null, new Pen(Brushes.AntiqueWhite, stroke), path);
            GridDrawing.Freeze();
            UpdateDrawings();
        }

        public void UpdateOverlayDrawing()
        {
            UpdateDrawings2();
            return;
            var path = new PathGeometry();

            var segments = new List<PathSegment>();

            var points = GetHexPoints(Height, Width);

            for (var i = 1; i < points.Length; i++)
            {
                var seg = new LineSegment(points[i], false);
                seg.Freeze();
                segments.Add(seg);
            }

            var poly =
                new PathFigure(
                    points[0],
                    segments,
                    true
                );

            poly.Freeze();
            path.Figures.Add(poly);
            path.Freeze();

            OverlayDrawing = new GeometryDrawing(HighlightBrush, new Pen(null, 0.0), path);
            OverlayDrawing.Freeze();
            UpdateDrawings();
        }

        private void UpdateDrawings()
        {
            return;
            //var dGroup = new DrawingGroup();

            //dGroup.Append();
            //dGroup.Children.Add(TerrainDrawing ?? BlankDrawing);
            //dGroup.Children.Add(GridDrawing ?? BlankDrawing);

            //if(HasHighlight)
            //    dGroup.Children.Add(OverlayDrawing ?? BlankDrawing);

            //if(HasUnit)
            //    dGroup.Children.Add(UnitDrawing ?? BlankDrawing);

            //dGroup.Freeze();

            //DirectCanvas.Background = new DrawingBrush(dGroup) { TileMode = TileMode.None };
        }

        private void UpdateDrawings2()
        {
            var brush = TileCache.Get(State, Scale, Height, Width, State.GetTerrain(Location), State.GetUnit(Location), Highlight);
            if (DirectCanvas.Fill != brush)
            {
                DirectCanvas.Fill = brush;
            }
        }

        private void UpdateHighlights()
        {
            BrushSet set = HighlightBrushSets[Highlight];

            if (HighlightBrush == set.OverlayBrush)
                return;

            HighlightBrush = set.OverlayBrush;

            HasHighlight = Highlight != HighlightMode.None;
            UpdateOverlayDrawing();
            //OverlayPoly.Fill = set.OverlayBrush;
            //GridOverlay.Stroke = set.BorderBrush;
        }


        #endregion

        #region Helpers

        private Point[] GetHexPoints(int height, int width)
        {
            return new Point[]
            {
                new Point(0, height / 2),
                new Point(width * 0.25, height),
                new Point(width * 0.75, height),
                new Point(width, height / 2),
                new Point(width * 0.75, 0),
                new Point(width * 0.25, 0)
            };
        }

        #endregion


        private class BrushSet
        {
            public Brush OverlayBrush { get; set; } = new SolidColorBrush(Colors.Transparent);
            public Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.AntiqueWhite);
        }

        public class TileMouseButtonEventArgs
        {
            public Location Location { get; }
            public MouseButtonEventArgs Args { get; }

            public TileMouseButtonEventArgs(Location location, MouseButtonEventArgs args)
            {
                Location = location;
                Args = args;
            }
        }

        public class TileMouseEventArgs
        {
            public Location Location { get; }
            public MouseEventArgs Args { get; }

            public TileMouseEventArgs(Location location, MouseEventArgs args)
            {
                Location = location;
                Args = args;
            }
        }

        public class TileChainMouseButtonEventArgs
        {
            public ActionChain Chain { get; }
            public Location Location { get; }
            public MouseButtonEventArgs Args { get; }

            public TileChainMouseButtonEventArgs(ActionChain chain, Location location, MouseButtonEventArgs args)
            {
                Chain = chain;
                Location = location;
                Args = args;
            }
        }

        public class TileChainMouseEventArgs
        {
            public ActionChain Chain { get; }
            public Location Location { get; }
            public MouseEventArgs Args { get; }

            public TileChainMouseEventArgs(ActionChain chain, Location location, MouseEventArgs args)
            {
                Chain = chain;
                Location = location;
                Args = args;
            }
        }

        public class Void : Game.TerrainType
        {
            private Void() : base("void") { }
            public static Void Instance { get; } = new Void();

            public override TerrainClassifications Classification { get; } = TerrainClassifications.Land | TerrainClassifications.River | TerrainClassifications.Sea;

            public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Natural;

            public override TerrainHeight Height { get; } = TerrainHeight.Normal;
        }

        public class BitmapTileCache
        {
            private Dictionary<string, Brush> Cache { get; } = new Dictionary<string, Brush>();

            public Brush Get(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, HighlightMode highlightMode)
            {
                var unitEffectiveConcealment = unit?.GetEffectiveConcealment(state, terrain);
                var key = GetKey(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);
                Brush brush;
                if (Cache.TryGetValue(key, out brush))
                    return brush;


                var drawing = GetCacheBrush(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);

                //var visual = new Rectangle() { Height = height, Width = width, Fill = brush };
                //
                //var bitmap = new System.Windows.Media.Imaging.RenderTargetBitmap((int)Math.Ceiling(width), (int)Math.Ceiling(height), 96, 96, PixelFormats.Pbgra32);
                var visual = new DrawingVisual();
                ////visual.CacheMode = new BitmapCache();
                using (var render = visual.RenderOpen())
                {
                    render.DrawDrawing(drawing);
                    render.Close();
                }

                var cache = new BitmapCacheBrush() { Target = visual, BitmapCache = new BitmapCache(), AutoLayoutContent = false };
                //bitmap.Render();
                //var cache = new ImageBrush(new DrawingImage(drawing));
                //var image = new Image() { Source = new DrawingImage(drawing) };
                //var cache = new DrawingBrush(drawing);
                Cache.Add(key, cache);

                return cache;
            }

            private Drawing GetCacheBrush(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, HighlightMode highlightMode)
            {
                var group = new DrawingGroup();
                group.Append();
                group.Children.Add(GetTerrainDrawing(state, scale, height, width, terrain, unit, highlightMode));
                group.Children.Add(GetGridDrawing(height, width));
                //var overlayDrawing = GetOverlayDrawing(height, width, highlightMode);
                //group.Children.Add(overlayDrawing);
                var unitDrawing = GetUnitDrawing(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);
                if(null != unitDrawing)
                    group.Children.Add(unitDrawing);
                group.Freeze();

                return group;
            }

            private string GetKey(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, HighlightMode highlightMode)
            {
                var sb = new StringBuilder();
                sb.Append(scale);
                sb.Append("|");
                sb.Append(height);
                sb.Append("|");
                sb.Append(width);
                sb.Append("|");
                sb.Append(terrain.IsOwned);
                sb.Append("|");
                sb.Append(terrain.CommanderID);
                sb.Append("|");
                sb.Append(terrain.DigIn);
                sb.Append("|");
                sb.Append(terrain.Health);
                sb.Append("|");
                sb.Append(terrain.TerrainType.Key);
                sb.Append("|");
                sb.Append(unit?.Health.ToString() ?? "null");
                sb.Append("|");
                sb.Append(unitEffectiveConcealment?.ToString() ?? "null");
                sb.Append("|");
                sb.Append(unit?.CommanderID.ToString() ?? "null");
                sb.Append("|");
                sb.Append(unit?.UnitType.Key ?? "null");
                sb.Append("|");
                sb.Append(highlightMode.ToString());
                return sb.ToString();
            }

            private Point[] GetHexPoints(double height, double width)
            {
                return new Point[]
                {
                new Point(0, height / 2),
                new Point(width * 0.25, height),
                new Point(width * 0.75, height),
                new Point(width, height / 2),
                new Point(width * 0.75, 0),
                new Point(width * 0.25, 0)
                };
            }

            private Drawing GetOverlayDrawing(double height, double width, HighlightMode highlightMode)
            {
                Brush highlightBrush;
                switch (highlightMode)
                {
                    case HighlightMode.Ally:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                        break;
                    case HighlightMode.Enemy:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                        break;
                    case HighlightMode.Neutral:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
                        break;
                    case HighlightMode.None:
                        return null;
                    case HighlightMode.Selected:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200));
                        break;
                    case HighlightMode.SelectionAvailable:
                        return null;
                    case HighlightMode.SelectionUnavailable:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
                        break;
                    default:
                        throw new ArgumentException($"Unknown highlight mode of {highlightMode}");
                }

                var path = new PathGeometry();

                var segments = new List<PathSegment>();

                var points = GetHexPoints(height, width);

                for (var i = 1; i < points.Length; i++)
                {
                    var seg = new LineSegment(points[i], false);
                    seg.Freeze();
                    segments.Add(seg);
                }

                var poly =
                    new PathFigure(
                        points[0],
                        segments,
                        true
                    );

                poly.Freeze();
                path.Figures.Add(poly);
                path.Freeze();

                var drawing = new GeometryDrawing(highlightBrush, new Pen(null, 0.0), path);
                drawing.Freeze();
                return drawing;                
            }

            private Drawing GetGridDrawing(double height, double width)
            {
                var path = new PathGeometry();

                var segments = new List<PathSegment>();

                var stroke = 4.0;
                var halfStroke = 2.0;

                var points = new Point[]
                {
                new Point(0 - halfStroke, height / 2),
                new Point(width * 0.25 - halfStroke, height + halfStroke),
                new Point(width * 0.75 + halfStroke, height + halfStroke),
                new Point(width + halfStroke, height / 2),
                new Point(width * 0.75 + halfStroke, 0 - halfStroke),
                new Point(width * 0.25 - halfStroke, 0 - halfStroke)
                };

                for (var i = 1; i < points.Length; i++)
                {
                    var seg = new LineSegment(points[i], true);
                    seg.Freeze();
                    segments.Add(seg);
                }

                var poly =
                    new PathFigure(
                        points[0],
                        segments,
                        true
                    );

                poly.Freeze();
                path.Figures.Add(poly);

                //var fmTxt = new FormattedText(Location.ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 24, Brushes.Black);
                //path.AddGeometry(fmTxt.BuildGeometry(new Point(width / 2, height / 2)));

                path.Freeze();

                var drawing = new GeometryDrawing(null, new Pen(Brushes.AntiqueWhite, stroke), path);
                drawing.Freeze();
                return drawing;
            }

            private Drawing GetTerrainDrawing(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, HighlightMode highlightMode)
            {
                Brush brush;
                if (terrain.IsOwned)
                {
                    brush = Globals.GetCommanderColor(terrain.CommanderID);
                }
                else
                {
                    if (terrain.TerrainType.CanCapture)
                        brush = Resource.NPC_BRUSH;
                    else
                        brush = Resource.CANNOT_BE_CAPTURED_BRUSH;

                }

                Drawing drawing = null;
                TerrainBase baseUI;
                if (TerrainBase.TYPES.TryGetValue(terrain.TerrainType, out baseUI))
                {
                    drawing = baseUI.GetVisualization(terrain, height, width, Brushes.Gray, brush, new Pen(Brushes.Black, 1));
                }

                return drawing;
            }

            private Drawing GetUnitDrawing(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, HighlightMode highlightMode)
            {
                Drawing drawing = null;
                if (unit != null)
                {
                    UnitBase baseUI;
                    if (UnitBase.TYPES.TryGetValue(unit.UnitType, out baseUI))
                    {
                        drawing = baseUI.GetVisualization(UnitBase.Identifier.Friendly, (unit.Health != unit.UnitType.MaxHealth ? unit.Health.ToString() : null), unitEffectiveConcealment.ToString(), height, width, Globals.GetCommanderColor(unit.CommanderID));
                    }
                }

                return drawing;
            }

            public class Tile
            {
                public Brush GridDrawing { get; }
                public Brush TerrainDrawing { get; }
                public Brush UnitDrawing { get; }
            }
        }
    }
}
