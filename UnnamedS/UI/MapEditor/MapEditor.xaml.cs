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
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for MapEditor.xaml
    /// </summary>
    public partial class MapEditor : Page
    {
        private double Scale { get; set; } = 0.12;
        private double XOffset { get; set; }
        private double YOffset { get; set; }

        private EditorStateFields State { get; set; }
        private Dictionary<Location, UI.Tile> Tiles { get; } = new Dictionary<Location, Tile>();

        private TerrainType PlacementTerrainType { get; set; }
        private UnitType PlacementUnitType { get; set; }
        private int? PlacementCommanderID { get; set; }

        public MapEditor()
        {
            InitializeComponent();
        }

        private void LoadTiles()
        {
            Tiles.Clear();
            mapCanvas.Children.Clear();
            foreach (var terrain in State.Terrain)
            {
                var tile = new Tile(new Tile.CanvasSet(mapCanvas), State, terrain.Key, Scale);
                tile.MouseDown += Tile_MouseDown;
                tile.MouseEnter += Tile_MouseEnter;
                Tiles.Add(terrain.Key, tile);
            }

        }

        private void UpdateCommanders()
        {
            lblCommanders.Content = State?.Commanders.Count ?? 0;
            placementModeSelect.ValidCommanderIDs = State.Commanders.Select(c => c.CommanderID).ToList();
        }

        private void UpdatePlacementModeDisplay()
        {
            if (PlacementCommanderID.HasValue)
                lblPlacementMode.Background = Globals.GetCommanderColor(PlacementCommanderID.Value);
            else
                lblPlacementMode.Background = Brushes.Transparent;

            if (PlacementUnitType != null)
                lblPlacementMode.Content = $"Unit: {Globals.GetResource(PlacementUnitType.Key)}";
            else if (PlacementTerrainType != null)
                lblPlacementMode.Content = $"Terrain: {Globals.GetResource(PlacementTerrainType.Key)}";
            else
                lblPlacementMode.Content = "None";
        }

        private bool MouseDownOnTile = false;
        private void Tile_MouseDown(object sender, Tile.TileMouseButtonEventArgs e)
        {
            Place(e.Location);
            MouseDownOnTile = true;
        }

        private void Tile_MouseEnter(object sender, Tile.TileMouseEventArgs e)
        {
            if (MouseDownOnTile)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                    Place(e.Location);
                else
                    MouseDownOnTile = false;
            }
        }

        private void Place(Location location)
        {
            Place(location, PlacementZone.None, PlacementZone.None);
        }

        private void Place(Location location, PlacementZone hzZone, PlacementZone vtZone)
        {
            int? commanderID = PlacementCommanderID;

            if(hzZone == PlacementZone.None && vtZone == PlacementZone.None)
            {
                PlacementZone ihzZone;
                PlacementZone ivtZone;
                Location hzLoc = new Location(GetMidwayInfo(State.Width, location.X, out ihzZone), location.Y);
                Location vtloc = new Location(location.X, GetMidwayInfo(State.Height, location.Y, out ivtZone));
                if (optMirrorHorizontally.IsChecked == true)
                {
                    Place(hzLoc, ihzZone, ivtZone);
                }
                if(optMirrorVertically.IsChecked == true)
                {
                    Place(vtloc, ihzZone, ivtZone);
                }
            }
            else if(optMirrorCommanders.IsChecked == true && commanderID.HasValue)
            {
                if (hzZone == PlacementZone.InMid || vtZone == PlacementZone.InMid) {
                    commanderID = PlacementCommanderID;
                }
                else
                {
                    int overallCommanderAdd = 0;

                    PlacementZone compareZone = PlacementZone.None;

                    if (hzZone == PlacementZone.None)
                        compareZone = vtZone;
                    else if (vtZone == PlacementZone.None)
                        compareZone = hzZone;

                    if (compareZone != PlacementZone.None)
                    {
                        if (compareZone == PlacementZone.BeforeMid)
                            overallCommanderAdd = 0;
                        else
                            overallCommanderAdd = 1;
                    }
                    else if (vtZone == PlacementZone.BeforeMid && hzZone == PlacementZone.BeforeMid)
                        overallCommanderAdd = 0;
                    else if (vtZone == PlacementZone.BeforeMid && hzZone == PlacementZone.AfterMid)
                        overallCommanderAdd = 1;
                    else if (vtZone == PlacementZone.AfterMid && hzZone == PlacementZone.BeforeMid)
                        overallCommanderAdd = 2;
                    else
                        overallCommanderAdd = 3;


                    commanderID = State.Commanders[overallCommanderAdd % State.Commanders.Count].CommanderID;
                }
            }

            if (PlacementTerrainType != null)
            {
                State.Terrain[location] = new Terrain(PlacementTerrainType, location, null, commanderID.HasValue, commanderID.HasValue ? commanderID.Value : -1);
                Tiles[location].UpdateTerrain();
            }
            else if (PlacementUnitType != null)
            {
                if (PlacementUnitType == RemoveUnitType.Instance)
                {
                    State.Units.Remove(location);
                }
                else
                {
                    State.Units[location] = new Unit(State.Units.Count == 0 ? 0 : State.Units.Max(kp => kp.Value.UnitID) + 1, PlacementUnitType, location, commanderID.Value);
                }
                Tiles[location].UpdateUnit();
            }
            else
            {
                MessageBox.Show("No Placement Mode Selected: Please select a placement mode from the menu on the left", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetMidwayInfo(int max, int current, out PlacementZone zone)
        {
            var other = 0;
            // Is even
            if(max % 2 == 0)
            {
                var half = max / 2;

                if (current < half)
                {
                    other = max - current - 1;
                    zone = PlacementZone.BeforeMid;
                }
                else
                {
                    other = max - current - 1;
                    zone = PlacementZone.AfterMid;
                }
            }
            else
            {
                var mid = (max - 1) / 2;

                if (current == mid)
                {
                    other = current;
                    zone = PlacementZone.InMid;
                }
                else if (current < mid)
                {
                    other = max - current - 1;
                    zone = PlacementZone.BeforeMid;
                }
                else
                {
                    other = max - current - 1;
                    zone = PlacementZone.AfterMid;
                }
            }

            return other;
        }

        private enum PlacementZone { None, BeforeMid, AfterMid, InMid }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            mapSizeEditorBorder.Visibility = Visibility.Visible;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var loadedWrapper = (Saving.Wrappers.Map)Saving.BattleSaving.LoadBattleGameState(new List<Saving.Wrappers.BaseWrapper>(1) { Saving.Wrappers.Map.Instance });

            if (loadedWrapper != null)
            {
                optAuthor.Text = loadedWrapper.Author;
                State = new EditorStateFields(loadedWrapper.Fields);
                UpdateCommanders();
                LoadTiles();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(State == null)
            {
                MessageBox.Show("Nothing to save", "Nothing to save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Saving.BattleSaving.SaveBattleGameState(new Saving.Wrappers.Map(State.ToFields(), optAuthor.Text));
        }

        private void CommanderAddButton_Click(object sender, RoutedEventArgs e)
        {
            State.Commanders.Add(new Commander(Game.CommanderTypes.BasicCommander.Instance, (State.Commanders.Count > 0 ? State.Commanders.Max(c => c.CommanderID) + 1 : 0)));
            UpdateCommanders();
        }

        private void CommanderRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Removing a Commander will REMOVE all of their units and turn all of the Terrain they own to NPC control. Are you sure you want remove a commander? This cannot be undone.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                return;

            var targetedID = State.Commanders.Max(c => c.CommanderID);
            State.Commanders.RemoveAll(c => c.CommanderID == targetedID);
            foreach(var terrain in State.Terrain.Values.ToList())
            {
                if(terrain.IsOwned && terrain.CommanderID == targetedID)
                {
                    State.Terrain[terrain.Location] = new Terrain(terrain.TerrainType, terrain.Location);
                    Tiles[terrain.Location].UpdateTerrain();
                }
            }

            foreach (var unit in State.Units.Values.ToList())
            {
                if (unit.CommanderID == targetedID)
                {
                    State.Units.Remove(unit.Location);
                    Tiles[unit.Location].UpdateUnit();
                }
            }
            UpdateCommanders();
        }

        private void PlacementModeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            placementModeSelectBorder.Visibility = Visibility.Visible;
        }

        private void placementModeSelect_ModeSelected(object sender, MapEditorPlacementModeEditor.ModeSelectedEventArgs e)
        {
            PlacementTerrainType = e.TerrainType;
            PlacementUnitType = e.UnitType;
            PlacementCommanderID = e.CommanderID;
            UpdatePlacementModeDisplay();
            placementModeSelectBorder.Visibility = Visibility.Collapsed;
        }

        private void placementModeSelect_Cancel(object sender, EventArgs e)
        {
            placementModeSelectBorder.Visibility = Visibility.Collapsed;
        }

        #region Copy & Paste

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.PreviewMouseDown += UI_MouseDown;
            window.PreviewMouseUp += UI_MouseUp;
            window.PreviewKeyDown += UI_KeyDown;
            window.PreviewKeyUp += UI_KeyUp;

            placementModeSelect.Load(Scale);

            StartGameLoopTimer();
            //GameLoop().ContinueWith(t => { GameLoopException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(false);
        }

        private System.Threading.Timer GameLoopTimer { get; set; }

        private long lastTickAt;
        private TimeSpan interval = new TimeSpan(10000);
        private void StartGameLoopTimer()
        {
            lastTickAt = DateTime.UtcNow.Ticks;

            GameLoopTimer = new System.Threading.Timer(GameLoopTimer_Tick, null, new TimeSpan(10000), new TimeSpan(-1));
        }

        private void GameLoopTimer_Tick(object sender)
        {
            long currentTickAt = DateTime.UtcNow.Ticks;
            double modifier = (currentTickAt - lastTickAt) / interval.Ticks;
            lastTickAt = currentTickAt;

            HandleContinuousInput(modifier * 0.04);

            GameLoopTimer.Change(interval, new TimeSpan(-1));
        }

        private static readonly double X_SCROLL_MODIFIER = 50.0;
        private static readonly double Y_SCROLL_MODIFIER = X_SCROLL_MODIFIER * (Resource.BITMAP_HIT_REFERENCE.PixelHeight / (double)Resource.BITMAP_HIT_REFERENCE.PixelWidth);

        private void HandleContinuousInput(double modifier)
        {
            var bindings = Settings.Settings.Current.InputBindings;

            //ActiveInput.RemoveAll(i => i.IsActive() == false);

            var oldYOffset = YOffset;
            var oldXOffset = XOffset;
            if (bindings.MoveUp.Active(ActiveInput))
            {
                YOffset -= Y_SCROLL_MODIFIER * modifier;
            }
            if (bindings.MoveDown.Active(ActiveInput))
            {
                YOffset += Y_SCROLL_MODIFIER * modifier;
            }
            if (bindings.MoveLeft.Active(ActiveInput))
            {
                XOffset -= X_SCROLL_MODIFIER * modifier;
            }
            if (bindings.MoveRight.Active(ActiveInput))
            {
                XOffset += X_SCROLL_MODIFIER * modifier;
            }


            if (oldYOffset != YOffset || oldXOffset != XOffset)
            {
                Dispatcher.Invoke(delegate
                {
                    mapCanvasContainer.RenderTransform = new TranslateTransform(-XOffset, -YOffset);
                    //Canvas.SetTop(mapCanvas, -YOffset);
                    //Canvas.SetLeft(mapCanvas, -XOffset);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private List<Settings.Input> ActiveInput = new List<Settings.Input>();

        private void UI_KeyUp(object sender, KeyEventArgs e)
        {
            var keyToRemove = new Settings.KeyInput(e.Key);

            ActiveInput.RemoveAll(i => keyToRemove.Equals(i));
        }

        private void UI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var mouseToRemove = new Settings.MouseInput(e.ChangedButton);

            ActiveInput.RemoveAll(i => mouseToRemove.Equals(i));
        }

        private void UI_KeyDown(object sender, KeyEventArgs e)
        {
            var keyToAdd = new Settings.KeyInput(e.Key);
            if (ActiveInput.Contains(keyToAdd) == false)
            {
                ActiveInput.Add(keyToAdd);
            }
        }

        private void UI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseToAdd = new Settings.MouseInput(e.ChangedButton);
            if (ActiveInput.Contains(mouseToAdd) == false)
            {
                ActiveInput.Add(mouseToAdd);
            }
        }

        #endregion

        private class EditorStateFields : IReadOnlyBattleGameState
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public Dictionary<Location, Unit> Units { get; } = new Dictionary<Location, Unit>();
            public Dictionary<Location, Terrain> Terrain { get; } = new Dictionary<Location, Terrain>();
            public List<Commander> Commanders { get; } = new List<Commander>();

            IReadOnlyDictionary<int, Unit> IReadOnlyBattleGameState.Units
            {
                get
                {
                    return Units.ToDictionary(kp => kp.Value.UnitID, kp => kp.Value);
                }
            }

            IReadOnlyCollection<Terrain> IReadOnlyBattleGameState.Terrain
            {
                get
                {
                    return Terrain.Values.ToList();
                }
            }

            IReadOnlyDictionary<int, Commander> IReadOnlyBattleGameState.Commanders
            {
                get
                {
                    return Commanders.ToDictionary(kp => kp.CommanderID, kp => kp);
                }
            }

            public int TurnID
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int CreditsPerCity
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Commander CurrentCommander
            {
                get
                {
                    return null;
                }
            }

            public int VictoryPointsPerPoint
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointLimitEnabled
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int VictoryPointLimit
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointGapEnabled
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int VictoryPointGap
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IReadOnlyList<GameAction> Actions
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool VictoryPointVictoryAchieved
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public EditorStateFields(int height, int width)
            {
                Height = height;
                Width = width;
            }

            public EditorStateFields(BattleGameState.Fields fields)
            {
                Height = fields.Height;
                Width = fields.Width;

                foreach(var terrain in fields.Terrain)
                {
                    Terrain.Add(terrain.Location, terrain);
                }
                foreach(var unit in fields.Units)
                {
                    Units.Add(unit.Location, unit);
                }
                foreach(var commander in fields.Commanders)
                {
                    Commanders.Add(commander);
                }
            }

            public BattleGameState.Fields ToFields()
            {
                var terrain = new Terrain[Terrain.Count];

                foreach(var t in Terrain.Values)
                {
                    terrain[BattleGameState.GetIndex(t.Location.X, t.Location.Y, Height)] = t;
                }

                return new BattleGameState.Fields(Height, Width, terrain, Units.Values.ToArray(), Commanders.ToArray(), new Dictionary<string, object>(0), -1);
            }

            public int GetNextUnitID()
            {
                throw new NotImplementedException();
            }

            public Unit GetUnit(Location loc)
            {
                Unit unit;
                if (Units.TryGetValue(loc, out unit))
                    return unit;
                else
                    return null;
            }

            public Unit GetUnit(int x, int y)
            {
                return GetUnit(new Location(x, y));
            }

            public Unit GetUnit(int unitId)
            {
                return Units.FirstOrDefault(u => u.Value.UnitID == unitId).Value;
            }

            public Terrain GetTerrain(Location loc)
            {
                Terrain terrain;
                if (Terrain.TryGetValue(loc, out terrain))
                    return terrain;
                else
                    return null;
            }

            public Terrain GetTerrain(int x, int y)
            {
                return GetTerrain(new Location(x, y));
            }

            public Game.Tile GetTile(Location loc)
            {
                return new Game.Tile(GetTerrain(loc), GetUnit(loc));
            }

            public Game.Tile GetTile(int x, int y)
            {
                return GetTile(new Location(x, y));
            }

            public Commander GetCommander(int commanderID)
            {
                return Commanders.FirstOrDefault(c => c.CommanderID == commanderID);
            }

            public Commander SafeGetCommander(int commanderID)
            {
                return GetCommander(commanderID);
            }

            public bool IsCommanderFriendly(int ourCommanderID, int otherCommanderID)
            {
                return ourCommanderID == otherCommanderID;
            }

            public bool LocationsAdjacent(Location a, Location b)
            {
                throw new NotImplementedException();
            }

            public List<Location> LocationsAroundPoint(Location point, int minimum, int maximum)
            {
                throw new NotImplementedException();
            }

            public List<Location> LocationsAroundPoint(Location point, int range)
            {
                throw new NotImplementedException();
            }

            public BattleGameState Fork()
            {
                throw new NotImplementedException();
            }

            public BattleGameState.Fields GetFields()
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, object> GetProperties()
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, object> GetWriteableProperties()
            {
                throw new NotImplementedException();
            }

            public void SetProperties(IDictionary<string, object> values)
            {
                throw new NotImplementedException();
            }
        }

        public class RemoveUnitType : UnitType
        {
            private RemoveUnitType() : base("SPECIAL_remove_unit_type") { }
            public static RemoveUnitType Instance { get; } = new RemoveUnitType();

            public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>(0);

            public override int BuildCost { get; } = 0;

            public override int Concealment { get; } = 0;

            public override double MaxArmor { get; } = 0;

            public override int MaxMovement { get; } = 0;

            public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>(0);

            public override MovementType MovementType { get; } = Game.MovementTypes.Propeller.Instance;

            public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>(0);

            public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>(0);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Confirm Back?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
                Resource.MainWindow.SetContent(new MainMenu());
        }

        private void mapSizeEditor_Saved(object sender, EventArgs e)
        {
            var i = mapSizeEditor.MapHeight * mapSizeEditor.MapWidth;
            var terrain = new List<Terrain>(i);

            for(var h = 0; h < mapSizeEditor.MapHeight; h++)
            {
                for (var w = 0; w < mapSizeEditor.MapWidth; w++)
                {
                    terrain.Add(new Terrain(Game.TerrainTypes.Plain.Instance, new Location(w, h)));
                }
            }

            State = new EditorStateFields(new BattleGameState.Fields(mapSizeEditor.MapHeight, mapSizeEditor.MapWidth, terrain.ToArray(), new Unit[0], new Commander[0], new Dictionary<string, object>(0), -1));
            UpdateCommanders();
            LoadTiles();
            mapSizeEditorBorder.Visibility = Visibility.Collapsed;
        }

        private void mapSizeEditor_Closed(object sender, EventArgs e)
        {
            mapSizeEditorBorder.Visibility = Visibility.Collapsed;
        }
    }
}
