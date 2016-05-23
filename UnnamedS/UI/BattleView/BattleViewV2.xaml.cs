using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
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
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.UI.TileUI;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for BattleViewV2.xaml
    /// </summary>
    public partial class BattleViewV2 : Page, IUserLogic
    {
        public SharedTileData SharedTileDataCache { get; } = new SharedTileData();

        public double Scale { get; private set; } = 0.12;
        public double XOffset { get; private set; } = 0.0;
        public double YOffset { get; private set; } = 0.0;

        public GameLogic Logic { get; private set; }
        public IReadOnlyBattleGameState State { get { return Logic?.State; } }
        public User OurUser { get; private set; }
        public bool OurTurn { get; private set; }

        private LoadingState _loadingStatus = LoadingState.Loaded;
        private LoadingState LoadingStatus
        {
            get { return _loadingStatus; }
            set
            {
                _loadingStatus = value;
                UpdateLoadingStatusDisplay();
                InvalidateVisual();
                Dispatcher.Invoke(new Action(() => { }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private Dictionary<Location, UI.Tile> Tiles { get; } = new Dictionary<Location, Tile>();

        private static readonly bool NETWORKED = Environment.GetCommandLineArgs().Contains("net");
        private static readonly bool SERVER = Environment.GetCommandLineArgs().Contains("server");

        private static readonly double ZOOM_MODIFIER = 0.02;
        private static readonly double X_SCROLL_MODIFIER = 50.0;
        private static readonly double Y_SCROLL_MODIFIER = X_SCROLL_MODIFIER * (Resource.BITMAP_HIT_REFERENCE.PixelHeight / (double)Resource.BITMAP_HIT_REFERENCE.PixelWidth);
        private static readonly double X_SCROLL_BORDER = 100.0;
        private static readonly double Y_SCROLL_BORDER = X_SCROLL_BORDER;

        private bool Shutdown { get; set; } = false;

        private List<Settings.Input> ActiveInput = new List<Settings.Input>();

        private ActionInfo TriggeredActionPendingInputActionInfo { get; set; }

        private enum ViewMode { Normal, TileSelect };

        private ViewMode _mode = ViewMode.Normal;
        private ViewMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                UpdateViewMode();
            }
        }

        private int? _selectedUnitID;
        private int? SelectedUnitID
        {
            get { return _selectedUnitID; }
            set
            {
                if(_selectedUnitID != value)
                {
                    _selectedUnitID = value;

                    if(_selectedUnitID != null)
                        SelectedTerrainLocation = null;

                    UpdateSelectedUnit();
                }
            }
        }

        private Location _selectedTerrainLocation;
        private Location SelectedTerrainLocation
        {
            get { return _selectedTerrainLocation; }
            set
            {
                if(_selectedTerrainLocation != value)
                {
                    _selectedTerrainLocation = value;

                    if(_selectedTerrainLocation != null)
                        SelectedUnitID = null;

                    UpdateSelectedTerrain();
                }
            }
        }

        private Dictionary<Location, List<ActionChain>> SelectedActionList { get; set; }
        private Dictionary<Location, ActionChain> TileSelectActionList { get; set; }

        private ActionViewV2 selectedItemActionView { get; set; }
        private ActionViewV2 currentCommanderActionView { get; set; }

        private System.Threading.Timer GameLoopTimer { get; set; }

        public BattleViewV2()
        {
            InitializeComponent();
            unitHoveredStatePropView.View = this;
            terrainHoveredStatePropView.View = this;
            commandersListing.View = this;
            userToCommanderAssignment.View = this;
        }

        #region Initialization

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.PreviewMouseDown += UI_MouseDown;
            window.PreviewMouseUp += UI_MouseUp;
            window.PreviewKeyDown += UI_KeyDown;
            window.PreviewKeyUp += UI_KeyUp;
            window.PreviewMouseMove += UI_MouseMove;

            BattleCanvas.PreviewMouseDown += BattleCanvas_PreviewMouseDown;

            BattleCanvas.ContextMenu = new ContextMenu();
            ContextMenuService.SetIsEnabled(BattleCanvas, false);

            //#region Generate Game for Testing
            //var logic = DebugGenerateGameLogic(this);

            //if (logic is LocalGameLogic)
            //{
            //    var user1 = new User(0, "First User", true);
            //    LoadLocalGame((logic as LocalGameLogic), new List<User>() {
            //        user1,
            //        new User(1, "Second User", true)
            //    }, user1);

            //    logic.AssignUserToCommander(user1.UserID, 0);
            //    logic.AssignUserToCommander(user1.UserID, 1);
            //}
            //else
            //{
            //    LoadNetworkedGame((logic as NetworkedGameLogic));
            //}
            //#endregion

            if (State != null)
                GenerateTiles();

            StartGameLoopTimer();
            //GameLoop().ContinueWith(t => { GameLoopException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(false);
        }

        public void LoadLocalGame(LocalGameLogic logic, List<User> users, User user)
        {
            LoadGame(logic);
            OurUser = user;

            foreach (var u in users)
                logic.AddUser(u, new List<IUserLogic> { this });
        }

        public void LoadNetworkedGame(NetworkedGameLogic logic)
        {
            Resource.MainWindow.viewer.AddSource("Client", logic.Client);
            LoadingStatus = LoadingState.Connecting;
            LoadGame(logic);

            logic.UserSetByServer += Networked_UserSetByServer;
            logic.Disconnected += Network_Disconnected;
            logic.NetworkException += Network_Exception;
            logic.SyncStarted += Network_SyncStarted;
            logic.SyncFinished += Network_SyncFinished;
            logic.Listen();
        }

        private void LoadGame(GameLogic logic)
        {
            Logic = logic;
            selectedItemActionView = new ActionViewV2(this, State);
            selectedItemActionViewContainer.Children.Clear();
            selectedItemActionViewContainer.Children.Add(selectedItemActionView);

            currentCommanderActionView = new ActionViewV2(this, State);
            currentCommanderActionViewContainer.Children.Clear();
            currentCommanderActionViewContainer.Children.Add(currentCommanderActionView);

            GenerateTiles();

            userToCommanderAssignmentGrid.Visibility = Visibility.Visible;
        }

        private void UpdateGame()
        {
            GenerateTiles();
        }

        private void GenerateTiles()
        {
            if (IsLoaded == false)
                return;

            var initState = LoadingStatus;

            BattleCanvas.Children.Clear();
            AllyOverlayCanvas.Children.Clear();
            EnemyOverlayCanvas.Children.Clear();
            TerrainOverlayCanvas.Children.Clear();
            NeutralOverlayCanvas.Children.Clear();
            SelectionUnavailableOverlayCanvas.Children.Clear();
            UnitExpendedOverlayCanvas.Children.Clear();
            Tiles.Clear();
            foreach (var terrain in State.Terrain)
            {
                var tile = new Tile(new Tile.CanvasSet(BattleCanvas, AllyOverlayCanvas, EnemyOverlayCanvas, TerrainOverlayCanvas, NeutralOverlayCanvas, SelectionUnavailableOverlayCanvas, UnitExpendedOverlayCanvas), State, terrain.Location, Scale) { View = this };
                tile.MouseDown += Tile_MouseDown;
                tile.MouseUp += Tile_MouseUp;
                tile.MouseEnter += Tile_MouseEnter;

                Tiles.Add(terrain.Location, tile);
            }

        }

        #endregion

        #region Updater Methods

        private void UpdateActionLists()
        {
            UpdateHighlights();
            UpdateSelectedActionList();
            UpdateCurrentCommanderActionList();
        }

        private void UpdateSelectedUnit()
        {
            UpdateHighlights();
            UpdateSelectedActionList();
        }

        private void UpdateSelectedTerrain()
        {
            UpdateHighlights();
            UpdateSelectedActionList();
        }

        private void UpdateCurrentCommander()
        {
            UpdateCurrentCommanderActionList();
        }

        private void UpdateUnitAt(Location location)
        {
            if (IsLoaded == false)
                return;

            Tile tile;

            if (Tiles.TryGetValue(location, out tile))
            {
                tile.UpdateUnit();

                if (SelectedUnitID != null)
                {
                    if (State.GetUnit(location)?.UnitID == SelectedUnitID)
                    {
                        UpdateSelectedUnit();
                    }
                }
            }
            else
            {
                if(State.GetUnit(location)?.Embarked == false)
                    throw new Game.Exceptions.StateMismatchException();
            }
        }

        private void UpdateTerrainAt(Location location)
        {
            if (IsLoaded == false)
                return;

            Tile tile;

            if (Tiles.TryGetValue(location, out tile))
            {
                tile.UpdateTerrain();

                if (SelectedTerrainLocation != null && location == SelectedTerrainLocation)
                {
                    UpdateSelectedTerrain();
                }
            }
            else
            {
                throw new Game.Exceptions.StateMismatchException();
            }
        }

        private void UpdateTurnInfo()
        {
            OurTurn = State.CurrentCommander != null && OurUser != null && Logic.IsUserCommanding(OurUser.UserID, State.CurrentCommander.CommanderID);

            UpdateHighlights();
            UpdateCurrentCommander();
            UpdateActionLists();
        }

        private void UpdateViewMode()
        {
            UpdateHighlights();
        }

        private void UpdateCurrentCommanderActionList()
        {

            if (OurTurn == false || State.CurrentCommander == null)
            {
                currentCommanderActionViewContainer.Visibility = Visibility.Collapsed;
                return;
            }

            currentCommanderActionViewContainer.Visibility = Visibility.Visible;

            var sourceContext = new CommanderContext(State.CurrentCommander.CommanderID);

            currentCommanderActionView.DisplayActions(State.CurrentCommander.CommanderType.Actions.Where(a => a.CanUserTrigger).ToList(), sourceContext, State.CurrentCommander.CommanderID);
        }

        private void UpdateHighlights()
        {
            var allyClip = new GeometryGroup();
            var enemyClip = new GeometryGroup();
            var terrainClip = new GeometryGroup();
            var neutralClip = new GeometryGroup();
            var selectionUnavaiableClip = new GeometryGroup();
            var unitExpendedClip = new GeometryGroup();

            if (Mode == ViewMode.Normal)
            {
                UpdateSelectedActionList();

                int? sourceCommanderID = null;

                if (SelectedUnitID != null)
                {
                    var unit = State.GetUnit(SelectedUnitID.Value);
                    sourceCommanderID = unit.CommanderID;
                }
                else if (SelectedTerrainLocation != null)
                {
                    var terrain = State.GetTerrain(SelectedTerrainLocation);

                    if (terrain.IsOwned)
                        sourceCommanderID = terrain.CommanderID;
                }

                foreach(var tile in Tiles.Values)
                {
                    var t = State.GetTile(tile.Location);
                    List<ActionChain> actionChains;
                    if (SelectedActionList.TryGetValue(tile.Location, out actionChains))
                    {
                        if (t.Unit != null)
                        {
                            if (sourceCommanderID != null && State.IsCommanderFriendly(sourceCommanderID.Value, t.Unit.CommanderID))
                            {
                                allyClip.Children.Add(tile.Clip); // = Tile.HighlightMode.Ally;
                            }
                            else
                            {
                                //tile.Highlight = Tile.HighlightMode.Enemy;
                                enemyClip.Children.Add(tile.Clip);
                            }
                        }
                        else if(actionChains[0].GetActions().Last() == Game.ActionTypes.ForUnits.Move.Instance)
                        {
                            //tile.Highlight = Tile.HighlightMode.Neutral;
                            neutralClip.Children.Add(tile.Clip);
                        }
                        else
                        {
                            terrainClip.Children.Add(tile.Clip);
                        }

                        //clip.Children.Add(tile.Clip);
                    }
                    if (t.Unit != null)
                    {
                        if (t.Unit.CommanderID != State.CurrentCommander.CommanderID)
                        {
                            unitExpendedClip.Children.Add(tile.UnitClip);
                        }
                        else if(Settings.Settings.Current.UnitExpendedHighlighting)
                        {
                            var hasAnyTargets = false;
                            foreach (var action in t.Unit.UnitType.Actions)
                            {

                                if (action.CanUserTrigger == false)
                                    continue;

                                if (action == Game.ActionTypes.ForUnits.ClearRepeatedActionManually.Instance || action == Game.ActionTypes.ForUnits.DigIn.Instance)
                                    continue;

                                var targets = action.ValidTargets(
                                     State,
                                     new ActionContext(
                                         State.CurrentCommander.CommanderID,
                                         ActionContext.TriggerAutoDetermineMode.ManuallyByUser,
                                         new UnitContext(t.Location),
                                         new NullContext()
                                    )
                                );

                                
                                foreach (var target in targets)
                                {
                                    hasAnyTargets = true;
                                    break;
                                }

                                if (hasAnyTargets == true)
                                    break;
                            }

                            if (hasAnyTargets == false)
                            {
                                unitExpendedClip.Children.Add(tile.UnitClip);
                            }
                        }
                    }
                }
            }
            else if(Mode == ViewMode.TileSelect)
            {
                foreach(var tile in Tiles.Values)
                {
                    ActionChain actionChain;

                    if(TileSelectActionList.TryGetValue(tile.Location, out actionChain))
                    {
                        //clip.Children.Add(tile.Clip);
                        //tile.Highlight = Tile.HighlightMode.SelectionAvailable;
                    }
                    else
                    {
                        //tile.Highlight = Tile.HighlightMode.SelectionUnavailable;
                        selectionUnavaiableClip.Children.Add(tile.Clip);
                    }
                }
            }

            allyClip.Freeze();
            enemyClip.Freeze();
            terrainClip.Freeze();
            neutralClip.Freeze();
            selectionUnavaiableClip.Freeze();
            unitExpendedClip.Freeze();

            AllyOverlayCanvas.Clip = allyClip;
            EnemyOverlayCanvas.Clip = enemyClip;
            TerrainOverlayCanvas.Clip = terrainClip;
            NeutralOverlayCanvas.Clip = neutralClip;
            SelectionUnavailableOverlayCanvas.Clip = selectionUnavaiableClip;
            UnitExpendedOverlayCanvas.Clip = unitExpendedClip;
        }

        private void UpdateSelectedActionList()
        {
            Location sourceLocation = null;
            SourceContext sourceContext = null;
            IReadOnlyList<ActionType> actions = null;
            Dictionary<Location, List<ActionChain>> actionChains = new Dictionary<Location, List<ActionChain>>();

            if (SelectedUnitID != null)
            {
                var unit = State.GetUnit(SelectedUnitID.Value);

                if(unit.CommanderID != State.CurrentCommander.CommanderID)
                {
                    selectedItemActionView.Visibility = Visibility.Collapsed;
                    SelectedActionList = actionChains;
                    return;
                }

                sourceContext = new UnitContext(unit.Location);
                actions = unit.UnitType.Actions;
                sourceLocation = unit.Location;
            }
            else if (SelectedTerrainLocation != null)
            {
                var terrain = State.GetTerrain(SelectedTerrainLocation);

                if (terrain.IsOwned && terrain.CommanderID != State.CurrentCommander.CommanderID)
                {
                    selectedItemActionView.Visibility = Visibility.Collapsed;
                    SelectedActionList = actionChains;
                    return;
                }

                sourceContext = new TerrainContext(terrain.Location);
                actions = terrain.TerrainType.Actions;
                sourceLocation = terrain.Location;
            }
            else
            {
                selectedItemActionView.Visibility = Visibility.Collapsed;
                SelectedActionList = actionChains;
                return;
            }
            selectedItemActionView.Visibility = Visibility.Visible;

            foreach (var action in actions.Where(a => a.CanUserTrigger == true && a.TargetValueTypes.Length > 0 && a.TargetValueTypes[0] == typeof(Location)))
            {
                foreach (var entry in action.ValidTargets
                        (
                            State,
                            new ActionContext(State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, sourceContext, new EmptyContext())
                        )
                    )
                {
                    if (entry is KeyValuePair<Location, ActionChain> == false)
                        throw new ArgumentException($"Expected action.ValidTargets to be of type KeyValuePair<Location, ActionChain>, not {entry.GetType()}");

                    var kp = (KeyValuePair<Location, ActionChain>)entry;

                    var location = kp.Key;

                    List<ActionChain> listing;
                    if (actionChains.TryGetValue(location, out listing) == false)
                    {
                        listing = new List<ActionChain>();
                        actionChains.Add(location, listing);
                    }

                    Contract.Assert(listing != null);

                    listing.Add(kp.Value);
                }
            }

            var displayActions = actions.Where(a =>
            {
                if (a.CanUserTrigger == false)
                    return false;

                if (a.TargetValueTypes.Length == 1 && a.TargetValueTypes[0] == typeof(Location))
                {
                    if (a.CanPerformOn(State, new ActionContext(State.CurrentCommander.CommanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, sourceContext, new GenericContext(sourceLocation))))
                        return true;
                    else
                        return false;
                }

                return true;
            });

            selectedItemActionView.DisplayActions(displayActions.ToList(), sourceContext, State.CurrentCommander.CommanderID);

            SelectedActionList = actionChains;
        }

        private void UpdateLoadingStatusDisplay()
        {
            switch(LoadingStatus)
            {
                case LoadingState.Loaded:
                    stateLoadingGrid.Visibility = Visibility.Collapsed;
                    return;
                case LoadingState.Connecting:
                    stateLoadingGrid.Visibility = Visibility.Visible;
                    stateLoadingStatusLabel.Content = "Connecting";
                    stateLoadingDetailedStatusLabel.Content = "Establishing connection to the server.";
                    return;
                case LoadingState.Syncing:
                    stateLoadingGrid.Visibility = Visibility.Visible;
                    stateLoadingStatusLabel.Content = "Syncing";
                    stateLoadingDetailedStatusLabel.Content = "Getting in-sync with the Server";
                    return;
                case LoadingState.Rendering:
                    stateLoadingGrid.Visibility = Visibility.Visible;
                    stateLoadingStatusLabel.Content = "Rendering";
                    stateLoadingDetailedStatusLabel.Content = "";
                    return;
            }
        }

        private void UpdateDisconnectedDisplay()
        {
            disconnectedContainer.Visibility = Visibility.Visible;
        }

        #endregion

        private long lastTickAt;
        private TimeSpan interval = new TimeSpan(20000);
        private void StartGameLoopTimer()
        {
            lastTickAt = DateTime.UtcNow.Ticks;

            GameLoopTimer = new System.Threading.Timer(GameLoopTimer_Tick, null, interval, new TimeSpan(-1));
        }

        private void GameLoopTimer_Tick(object sender)
        {
            long currentTickAt = DateTime.UtcNow.Ticks;
            double modifier = (currentTickAt - lastTickAt) / interval.Ticks;
            lastTickAt = currentTickAt;

            HandleContinuousInput(modifier * 0.04);

            if (mouseIsDown)
            {
                Dispatcher.Invoke(() =>
                {
                    if (Mouse.LeftButton != MouseButtonState.Pressed)
                    {
                        mouseIsDown = false;
                        mouseDownAtX = 0;
                        mouseDownAtY = 0;
                        return;
                    }

                    var pos = Mouse.GetPosition(this);
                    var xDiff = pos.X - mouseDownAtX;
                    var yDiff = pos.Y - mouseDownAtY;

                    mouseDownAtDiffTrack += Math.Abs(xDiff) + Math.Abs(yDiff);

                    XOffset = xOffsetAtDown - xDiff;
                    YOffset = yOffsetAtDown - yDiff;

                    if (xDiff > 0 || yDiff > 0)
                        UpdateCanvasPosition();

                }, System.Windows.Threading.DispatcherPriority.Background);
                //BattleCanvas.Margin = new Thickness(-XOffset, -YOffset, 0, 0);
            }

            GameLoopTimer.Change(interval, new TimeSpan(-1));
        }

        private async Task GameLoop()
        {
            double interval = 10000 * 50;
            long lastTickAt = DateTime.UtcNow.Ticks;
            TimeSpan waitSpan;
            while (Shutdown == false)
            {
                long currentTickAt = DateTime.UtcNow.Ticks;
                double modifier = (currentTickAt - lastTickAt) / interval;

                HandleContinuousInput(modifier);

                lastTickAt = currentTickAt;
                waitSpan = new TimeSpan(DateTime.UtcNow.Ticks - currentTickAt);
                if (waitSpan.Ticks > 0)
                {
                    await Task.Delay(waitSpan);
                }
            }
        }

        #region Trigger Action

        private void InternalTriggerActions(List<ActionInfo> actions)
        {
            try
            {
                using (Dispatcher.DisableProcessing())
                {
                    Logic.DoActions(actions);
                }
            }
            catch (Game.Exceptions.ServerTooFarBehindException ex)
            {
                HandleServerTooFarBehind(ex);
            }
        }

        private void HandleServerTooFarBehind(Game.Exceptions.ServerTooFarBehindException ex)
        {
            Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Unable to perform action: {ex.Message}") ));
        }

        public void TriggerAction(ActionType actionType, int commanderID, SourceContext sourceContext, TargetContext targetContext)
        {
            Contract.Requires<ArgumentNullException>(null != actionType);
            Contract.Requires<ArgumentNullException>(null != sourceContext);
            Contract.Requires<ArgumentNullException>(null != targetContext);

            TriggeredActionPendingInputActionInfo = null;

            if (actionType.CheckTargetContext(targetContext))
            {
                InternalTriggerActions(new List<ActionInfo>() { new ActionInfo(actionType, new ActionContext(commanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, sourceContext, targetContext)) });
            }
            else if(sourceContext is TileContext && actionType.TargetValueTypes.Length == 1 && actionType.TargetValueTypes[0] == typeof(Location))
            {
                TriggerAction(actionType, commanderID, sourceContext, new GenericContext((sourceContext as TileContext).Location));
            }
            else
            {
                var context = new ActionContext(commanderID, ActionContext.TriggerAutoDetermineMode.ManuallyByUser, sourceContext, targetContext);

                TriggeredActionPendingInputActionInfo = new ActionInfo(actionType, context);
                TriggeredActionInputPrompt(actionType, actionType.ValidTargets(State, context));
            }
        }

        private void TriggeredActionInputPrompt(ActionType actionType, System.Collections.IEnumerable options)
        {
            if (options is IReadOnlyDictionary<Location, ActionChain>)
            {
                TileSelectActionList = (options as IReadOnlyDictionary<Location, ActionChain>).ToDictionary(kp => kp.Key, kp => kp.Value);
                Mode = ViewMode.TileSelect;
            }
            else if(actionType is Game.ActionTypes.ForTerrain.BuildUnit)
            {
                TriggeredActionFreeformBuildUnitFill(actionType, options);
                freeformInputPromptContainer.Visibility = Visibility.Visible;
            }
            else
            {
                inputPrompt.Options = options;
                inputPromptGrid.Visibility = Visibility.Visible;
            }
        }

        private void TriggeredActionCancel()
        {
            TriggeredActionPendingInputActionInfo = null;

            inputPromptGrid.Visibility = Visibility.Collapsed;
            freeformInputPromptContainer.Visibility = Visibility.Collapsed;
            TriggeredActionFreeformInputPromptClear();

            if (Mode == ViewMode.TileSelect)
                Mode = ViewMode.Normal;
        }

        private void TriggeredActionInputEntered(object input)
        {
            var type = TriggeredActionPendingInputActionInfo.Type;
            var context = TriggeredActionPendingInputActionInfo.Context;
            var values = context.Target.Values.ToList();
            values.Add(input);
            TriggeredActionCancel();
            TriggerAction(type, (int)context.TriggeredByCommanderID, context.Source, new GenericContext(values.ToArray()));
        }

        private void TriggeredActionFreeformBuildUnitFill(ActionType actionType, System.Collections.IEnumerable options)
        {
            var buildType = (actionType as Game.ActionTypes.ForTerrain.BuildUnit);

            var wrapPanel = new WrapPanel() { Orientation = Orientation.Vertical };

            var list = new HashSet<UnitType>();

            foreach(var unitType in options.Cast<UnitType>())
            {
                list.Add(unitType);
            }

            {
                var terrain = new Terrain(Tile.Void.Instance, SingleTileView.MockBattleGameState.Location, null);
                var commander = State.CurrentCommander;
                foreach (var unitType in buildType.BuildableTypes.OrderBy(u => u.BuildCost))
                {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());

                    var stv = new SingleTileView();
                    stv.Set(terrain, new Unit(0, unitType, SingleTileView.MockBattleGameState.Location, commander.CommanderID), commander, 0.2);
                    grid.Children.Add(stv);

                    var infoCardGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                    infoCardGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    infoCardGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    var row = 0;
                    Resource.GenerateRow(infoCardGrid, ref row, "Name", Globals.GetResource(unitType.Key));
                    Resource.GenerateRow(infoCardGrid, ref row, "Build Cost", $"{unitType.BuildCost:C0}");

                    var diff = unitType.BuildCost - commander.Credits;

                    if (diff > 0)
                        Resource.GenerateRow(infoCardGrid, ref row, "Credits Needed", $"{diff:C0}");

                    Grid.SetColumn(infoCardGrid, 1);
                    grid.Children.Add(infoCardGrid);

                    var button = new Button() { Content = grid, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch, Tag = unitType };
                    button.Click += FreeformButtonClick;
                    button.IsEnabled = list.Contains(unitType);
                    wrapPanel.Children.Add(button);
                }
            }

            freeformInputPromptGrid.Children.Add(new ScrollViewer() { Content = wrapPanel, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled });
        }

        private void FreeformButtonClick(object sender, RoutedEventArgs e)
        {
            TriggeredActionInputEntered((sender as Button).Tag);
        }

        private void TriggeredActionFreeformInputPromptClear()
        {
            freeformInputPromptGrid.ColumnDefinitions.Clear();
            freeformInputPromptGrid.RowDefinitions.Clear();
            freeformInputPromptGrid.Children.Clear();
        }

        #endregion



        #region Input Handling

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
                    UpdateCanvasPosition();
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void UpdateCanvasPosition()
        {
            ContainerCanvas.RenderTransform = new TranslateTransform(-XOffset, -YOffset);
            //OverlayCanvas.RenderTransform = new TranslateTransform(-XOffset, -YOffset);
            //Canvas.SetTop(BattleCanvas, -YOffset);
            //Canvas.SetLeft(BattleCanvas, -XOffset);
            //BattleCanvas.Margin = new Thickness(-XOffset, -YOffset, 0, 0);
        }

        private void HandleOneTimeInput()
        {
            var bindings = Settings.Settings.Current.InputBindings;

            //ActiveInput.RemoveAll(i => i.IsActive() == false);

            var oldScale = Scale;
            if (bindings.ZoomIn.Active(ActiveInput))
            {
                Scale += ZOOM_MODIFIER;
            }
            if (bindings.ZoomOut.Active(ActiveInput))
            {
                Scale -= ZOOM_MODIFIER;
            }

            if (oldScale != Scale)
            {
                foreach (var tile in Tiles)
                {
                    tile.Value.Scale = Scale;
                    tile.Value.UpdateScale();
                    tile.Value.UpdateOverlayDrawing();
                    tile.Value.UpdateGridDrawing();
                    tile.Value.UpdateTerrain();
                    tile.Value.UpdateUnit();
                }
                UpdateHighlights();
            }

            if(bindings.CancelSelection.Active(ActiveInput))
            {
                SelectedUnitID = null;
                SelectedTerrainLocation = null;
                TriggeredActionCancel();
            }
        }

        #endregion

        public bool WasDragging(MouseButtonEventArgs e)
        {
            var relPos = e.GetPosition(BattleCanvas);

            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released && IsDragging())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsDragging()
        {
            return mouseDownAtDiffTrack >= mouseDownAtMarginOfError;
        }

        public void CancelDragging()
        {
            mouseIsDown = false;
            mouseDownAtDiffTrack = 0;
        }

        #region Event Handlers

        #region UI Key-bindings

        private void UI_KeyUp(object sender, KeyEventArgs e)
        {
            var keyToRemove = new Settings.KeyInput(e.Key);

            ActiveInput.RemoveAll(i => keyToRemove.Equals(i));
        }

        private void UI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var mouseToRemove = new Settings.MouseInput(e.ChangedButton);

            ActiveInput.RemoveAll(i => mouseToRemove.Equals(i));

            if(e.ChangedButton == MouseButton.Left)
            {
                mouseDownAtX = 0;
                mouseDownAtY = 0;
                mouseIsDown = false;
            }
        }

        private void UI_KeyDown(object sender, KeyEventArgs e)
        {
            var keyToAdd = new Settings.KeyInput(e.Key);
            if (ActiveInput.Contains(keyToAdd) == false)
            {
                ActiveInput.Add(keyToAdd);
                HandleOneTimeInput();
            }
            //HandleContinuousInput(0.5);
        }

        private void UI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseToAdd = new Settings.MouseInput(e.ChangedButton);
            if (ActiveInput.Contains(mouseToAdd) == false)
            {
                ActiveInput.Add(mouseToAdd);
                HandleOneTimeInput();
            }
            //HandleContinuousInput(0.5);

            //if(e.ChangedButton == MouseButton.Left)
            //{
            //    var relPos = e.GetPosition(this);
            //    mouseDownAtX = relPos.X;
            //    mouseDownAtY = relPos.Y;
            //    xOffsetAtDown = XOffset;
            //    yOffsetAtDown = YOffset;
            //    mouseIsDown = true;
            //    mouseDownAtDiffTrack = 0;
                
            //}
        }


        private void BattleCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var relPos = e.GetPosition(this);
                mouseDownAtX = relPos.X;
                mouseDownAtY = relPos.Y;
                xOffsetAtDown = XOffset;
                yOffsetAtDown = YOffset;
                mouseIsDown = true;
                mouseDownAtDiffTrack = 0;

            }
        }


        private double xOffsetAtDown = 0;
        private double yOffsetAtDown = 0;
        private double mouseDownAtX = 0;
        private double mouseDownAtY = 0;
        private double mouseDownAtDiffTrack = 0;
        private bool mouseIsDown = false;
        private const double mouseDownAtMarginOfError = 30;
        private void UI_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                {
                    mouseIsDown = false;
                    mouseDownAtX = 0;
                    mouseDownAtY = 0;
                    return;
                }

                var pos = e.GetPosition(this);
                var xDiff = pos.X - mouseDownAtX;
                var yDiff = pos.Y - mouseDownAtY;

                mouseDownAtDiffTrack += Math.Abs(xDiff) + Math.Abs(yDiff);

                XOffset = xOffsetAtDown - xDiff;
                YOffset = yOffsetAtDown - yDiff;

                UpdateCanvasPosition();
                //BattleCanvas.Margin = new Thickness(-XOffset, -YOffset, 0, 0);
            }
        }

        #endregion

        #region UI Other

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            battleMenu.Visibility = Visibility.Visible;
        }

        private void battleMenu_SaveGame(object sender, EventArgs e)
        {
            if(Saving.BattleSaving.SaveBattleGameState(
                new Saving.Wrappers.SaveGame(State.GetFields(), 
                (Logic is LocalGameLogic ? Saving.Wrappers.SaveGame.GameMode.Local : Saving.Wrappers.SaveGame.GameMode.Networked))
              ) == true)
            {
                battleMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void battleMenu_ExitToMenu(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit to the main menu? All unsaved progress will be lost.", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if(Logic is NetworkedGameLogic)
                {
                    var networkLogic = (NetworkedGameLogic)Logic;
                    networkLogic.Client.Disconnect();
                    Resource.MainWindow.Server?.Stop();
                }
                Resource.MainWindow.SetContent(new MainMenu());
            }
        }

        private void battleMenu_ExitToDesktop(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to exit to desktop? All unsaved progress will be lost.", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void battleMenu_ReturnToBattle(object sender, EventArgs e)
        {
            battleMenu.Visibility = Visibility.Collapsed;
        }

        private void battleMenu_CommanderAssignment(object sender, EventArgs e)
        {
            battleMenu.Visibility = Visibility.Collapsed;
            userToCommanderAssignmentGrid.Visibility = Visibility.Visible;
        }

        private void userToCommanderAssignment_CommandTrigger(object sender, UserToCommanderAssignment.CommandUserLinkEventArgs e)
        {
            if (Logic is LocalGameLogic)
            {
                (Logic as LocalGameLogic).AssignUserToCommander(e.UserID, e.CommanderID, OurUser.UserID, OurUser.IsHost);
            }
            else if(Logic is NetworkedGameLogic)
            {
                (Logic as NetworkedGameLogic).AssignUserToCommander(e.UserID, e.CommanderID);
            }
            else
            {
                Contract.Assert(false);
            }
        }

        private void UserToCommanderAssignmentCloseButton_Click(object sender, RoutedEventArgs e)
        {
            userToCommanderAssignmentGrid.Visibility = Visibility.Collapsed;
        }


        private void UnitTypeViewEncyclopedia_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Help.HelpView.Instance.Back = this;
            Help.HelpView.Instance.Display(((sender as MenuItem).Tag as UnitType));
            Resource.MainWindow.SetContent(Help.HelpView.Instance);
        }

        private void TerrainTypeViewEncyclopedia_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Help.HelpView.Instance.Back = this;
            Help.HelpView.Instance.Display(((sender as MenuItem).Tag as TerrainType));
            Resource.MainWindow.SetContent(Help.HelpView.Instance);
        }

        #endregion

        #region Tiles

        private void Tile_MouseEnter(object sender, Tile.TileMouseEventArgs e)
        {
            var tile = State.GetTile(e.Location);
            unitHoveredStatePropView.Unit = tile.Unit;
            terrainHoveredStatePropView.Terrain = tile.Terrain;
        }

        private void Tile_MouseDown(object sender, Tile.TileMouseButtonEventArgs e)
        {

        }

        private void Tile_MouseUp(object sender, Tile.TileMouseButtonEventArgs e)
        {
            if(WasDragging(e.Args))
            {
                return;    
            }

            if(Mode == ViewMode.Normal)
            {
                if(e.Args.ChangedButton == MouseButton.Left)
                {
                    
                    if(SelectedActionList.ContainsKey(e.Location))
                    {
                        OpenContextMenu(e.Location);
                        //Logic.DoActions(actionChains.First().GetActionsInfo(State.CurrentCommander.CommanderID));
                    }
                    else
                    {
                        var tile = State.GetTile(e.Location);

                        if(State.CurrentCommander == null || Logic.IsUserCommanding(OurUser.UserID, State.CurrentCommander.CommanderID) == false)
                        {
                            SelectedUnitID = null;
                            OpenContextMenu(e.Location);
                        }
                        else if(tile.Unit != null && tile.Unit.CommanderID == State.CurrentCommander.CommanderID)
                        {
                            SelectedUnitID = tile.Unit.UnitID;
                        }
                        else if(tile.Terrain.IsOwned && tile.Terrain.CommanderID == State.CurrentCommander.CommanderID)
                        {
                            SelectedTerrainLocation = e.Location;
                        }
                        else
                        {
                            SelectedUnitID = null;

                            OpenContextMenu(e.Location);
                        }
                    }
                }
            }
            else if(Mode == ViewMode.TileSelect)
            {
                if(e.Args.ChangedButton == MouseButton.Left)
                {
                    if(TileSelectActionList.ContainsKey(e.Location))
                    {
                        TriggeredActionInputEntered(e.Location);
                    }
                    else
                    {
                        System.Media.SystemSounds.Exclamation.Play();
                    }

                    e.Args.Handled = true;
                }
            }
            else
            {
                throw new ArgumentException($"Unknown Mode {Mode}");
            }
        }

        private void Tile_MouseUpChain(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                InternalTriggerActions(((sender as MenuItem).Tag as ActionChain).GetActionsInfo(State.CurrentCommander.CommanderID));
        }

        private void Tile_ToolTipOpeningChain(object sender, ToolTipEventArgs e)
        {
            var chain = (sender as FrameworkElement).Tag as ActionChain;

            var frameworkElm = (sender as FrameworkElement);

            ToolTipService.SetShowDuration(frameworkElm, int.MaxValue);

            if (chain.Length == 1)
            {
                var lastActionInfo = chain.GetActionsInfo(State.CurrentCommander.CommanderID).Last();

                frameworkElm.ToolTip = new ActionDetails() { Modifiers = lastActionInfo.Type.Modifiers(State, lastActionInfo.Context) };
            }
            else
            {
                frameworkElm.ToolTip = "Details unavailable for multi-actions";
            }
        }

        #endregion

        private void OpenContextMenu(Location location)
        {
            

            BattleCanvas.ContextMenu.Items.Clear();

            List<ActionChain> chains;

            if (SelectedActionList.TryGetValue(location, out chains))
            {
                foreach (var chain in chains)
                {
                    var item = new MenuItem() { Header = string.Join(" > ", chain.GetActions().Select(c => Globals.GetResource(c.Key))), Tag = chain, ToolTip = "placeholder" };
                    item.PreviewMouseUp += Tile_MouseUpChain;
                    item.ToolTipOpening += Tile_ToolTipOpeningChain;

                    BattleCanvas.ContextMenu.Items.Add(item);
                }
            }
            else
            {
                chains = new List<ActionChain>(0);
            }

            var additionalMenuItems = new List<MenuItem>();
            {
                var unit = State.GetUnit(location);

                if (unit != null)
                {
                    var unitMenuItem = new MenuItem() { Header = $"Encyclopedia: {Globals.GetResource(unit.UnitType.Key)}", Tag = unit.UnitType };
                    unitMenuItem.PreviewMouseUp += UnitTypeViewEncyclopedia_PreviewMouseUp;
                    additionalMenuItems.Add(unitMenuItem);
                }

                var terrain = State.GetTerrain(location);

                var terrainMenuItem = new MenuItem() { Header = $"Encyclopedia: {Globals.GetResource(terrain.TerrainType.Key)}", Tag = terrain.TerrainType };
                terrainMenuItem.PreviewMouseUp += TerrainTypeViewEncyclopedia_PreviewMouseUp;
                additionalMenuItems.Add(terrainMenuItem);

            }

            if (additionalMenuItems.Count > 0 && chains.Count > 0)
                BattleCanvas.ContextMenu.Items.Add(new Separator());

            foreach (var item in additionalMenuItems)
                BattleCanvas.ContextMenu.Items.Add(item);


            BattleCanvas.ContextMenu.IsOpen = true;

        }

        #region Input Prompt

        private void inputPrompt_OptionSelected(object sender, InputPrompt.OptionSelectedEventArgs e)
        {
            inputPromptGrid.Visibility = Visibility.Collapsed;
            TriggeredActionInputEntered(e.SelectedOption);
        }

        private void inputPrompt_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TriggeredActionCancel();
        }

        #endregion

        #region Network

        private void Networked_UserSetByServer(object sender, NetworkedGameLogic.UserSetEventArgs e)
        {
            if (OurUser == null)
                OurUser = e.User;
            else
                throw new NotSupportedException("User has already been set");

            LoadingStatus = LoadingState.Syncing;
        }

        private void Network_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {
            //Resource.MainWindow.ShowMessage(e.Exception);
            UpdateDisconnectedDisplay();
        }

        private void Network_Exception(object sender, Network.ExceptionEventArgs e)
        {
            Resource.MainWindow.ShowMessage(e.Exception);
        }

        private void Network_SyncFinished(object sender, EventArgs e)
        {
            LoadingStatus = LoadingState.Loaded;
        }

        private void Network_SyncStarted(object sender, EventArgs e)
        {
            LoadingStatus = LoadingState.Syncing;
        }

        #endregion

        #region Game

        private void GameLoopException(AggregateException exception)
        {
            Dispatcher.Invoke(() => { throw exception; });
        }

        #endregion

        #region IUserLogic

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
            if(e.ChangeInfo.CommanderID == State.CurrentCommander.CommanderID)
                UpdateCurrentCommander();

            UpdateActionLists();
            UpdateHighlights();
            commandersListing.Update();
            userToCommanderAssignment.Update();
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            UpdateUnitAt(e.ChangeInfo.Location);

            if(e.ChangeInfo.LocationChanged)
                UpdateUnitAt(e.ChangeInfo.PreviousLocation);

            unitHoveredStatePropView.Update();

            if ((e.ChangeInfo.ChangeCause == Game.StateChanges.UnitStateChange.Cause.Destroyed || e.ChangeInfo.ChangeCause == Game.StateChanges.UnitStateChange.Cause.Removed) && e.ChangeInfo.UnitID == SelectedUnitID)
                SelectedUnitID = null;

            UpdateActionLists();
            UpdateHighlights();
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            UpdateTerrainAt(e.ChangeInfo.Location);

            terrainHoveredStatePropView.Update();

            UpdateActionLists();
            UpdateHighlights();
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            UpdateTurnInfo();
            commandersListing.Update();
            userToCommanderAssignment.Update();
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            unitHoveredStatePropView.Update();
            terrainHoveredStatePropView.Update();
            commandersListing.Update();
            userToCommanderAssignment.Update();
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            UpdateHighlights();
            unitHoveredStatePropView.Update();
            terrainHoveredStatePropView.Update();
            commandersListing.Update();
            userToCommanderAssignment.Update();
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            UpdateHighlights();
            UpdateTurnInfo();
            unitHoveredStatePropView.Update();
            terrainHoveredStatePropView.Update();
            commandersListing.Update();
            userToCommanderAssignment.Update();
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            TriggeredActionCancel();
            UpdateTurnInfo();
            UpdateGame();
            commandersListing.Update();
        }

        public void OnTurnChanged(object sender, TurnChangedEventArgs args)
        {
            UpdateTurnInfo();
            commandersListing.Update();
        }

        public void OnSync(object sender, SyncEventArgs args)
        {
            if(args.SyncID == Logic.LastSyncID)
            {
                TriggeredActionCancel();
                UpdateTurnInfo();
                UpdateGame();
                commandersListing.Update();
                userToCommanderAssignment.Update();
            }
        }

        public void OnException(object sender, Game.Event.ExceptionEventArgs e)
        {
            Resource.MainWindow.ShowMessage(e.Exception);
        }

        public void OnVictoryConditionAchieved(object sender, VictoryConditionAchievedEventArgs args)
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

            grid.Children.Add(new Label() { Content = "Winners List", FontWeight = FontWeights.Bold });

            var row = 1;
            
            foreach(var cmdId in args.ChangeInfo.Winners)
            {
                string text;
                Brush background;
                Resource.GetCommanderIDNameAndColor(Logic, cmdId, out text, out background);
                var label = new Label() { Content = text, Background = background };
                Grid.SetRow(label, row++);
                grid.Children.Add(label);
            }            

            Resource.MainWindow.ShowMessage(Globals.GetResource(args.ChangeInfo.WinConditionKey), grid);
        }

        #endregion

        #endregion Event Handlers


        public class SharedTileData
        {
            public double LastScale { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            
        }


        public enum LoadingState { Loaded, Connecting, Syncing, Rendering }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            battleMenu_SaveGame(sender, new EventArgs());
        }

        private void ExitToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            battleMenu_ExitToMenu(sender, new EventArgs());
        }

        private void ExitToDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            battleMenu_ExitToDesktop(sender, new EventArgs());
        }

        private void FreeformInputPromptCancelButton_Click(object sender, RoutedEventArgs e)
        {
            TriggeredActionCancel();
        }

#if OFF
        public class BitmapTileCache
        {
            private Canvas RootCanvas { get; }

            public BitmapTileCache(Canvas canvas)
            {
                RootCanvas = canvas;
            }

            public Brush Get(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, Tile.HighlightMode highlightMode)
            {

            }

            public Brush Load(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, Tile.HighlightMode highlightMode)
            {
                var unitEffectiveConcealment = unit?.GetEffectiveConcealment(state, terrain);
                var key = GetKey(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);
                Brush brush;
                if (RootCanvas.Resources.Contains(key))
                    return RootCanvas.Resources.FindName[key];


                brush = GetCacheBrush(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);

                var visual = new Rectangle() { Height = height, Width = width, Fill = brush };
                var cache = new BitmapCacheBrush();
                cache.BitmapCache = new BitmapCache();
                cache.Target = visual;

                RootCanvas.Resources.Add(key, cache);

                return brush;
            }

            private Brush GetCacheBrush(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, Tile.HighlightMode highlightMode)
            {
                var group = new DrawingGroup();
                group.Append();
                group.Children.Add(GetTerrainDrawing(state, scale, height, width, terrain, unit, highlightMode));
                group.Children.Add(GetGridDrawing(height, width));
                group.Children.Add(GetOverlayDrawing(height, width, highlightMode));
                var unitDrawing = GetUnitDrawing(state, scale, height, width, terrain, unit, unitEffectiveConcealment, highlightMode);
                if (null != unitDrawing)
                    group.Children.Add(unitDrawing);
                group.Freeze();

                var brush = new DrawingBrush(group);

                RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                return brush;
            }

            private string GetKey(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, Tile.HighlightMode highlightMode)
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

            private Drawing GetOverlayDrawing(double height, double width, Tile.HighlightMode highlightMode)
            {
                Brush highlightBrush;
                switch (highlightMode)
                {
                    case Tile.HighlightMode.Ally:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                        break;
                    case Tile.HighlightMode.Enemy:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                        break;
                    case Tile.HighlightMode.Neutral:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
                        break;
                    case Tile.HighlightMode.None:
                        highlightBrush = Brushes.Transparent;
                        break;
                    case Tile.HighlightMode.Selected:
                        highlightBrush = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200));
                        break;
                    case Tile.HighlightMode.SelectionAvailable:
                        highlightBrush = Brushes.Transparent;
                        break;
                    case Tile.HighlightMode.SelectionUnavailable:
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

            private Drawing GetTerrainDrawing(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, Tile.HighlightMode highlightMode)
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
                    drawing = baseUI.GetVisualization(height, width, Brushes.Gray, brush, new Pen(Brushes.Black, 1));
                }

                return drawing;
            }

            private Drawing GetUnitDrawing(IReadOnlyBattleGameState state, double scale, double height, double width, Terrain terrain, Unit unit, int? unitEffectiveConcealment, Tile.HighlightMode highlightMode)
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
        }
#endif
    }
}
