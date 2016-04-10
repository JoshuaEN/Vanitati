using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.ExceptionServices;
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
using System.Windows.Threading;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for BattleView.xaml
    /// </summary>
    public partial class BattleView : Page, IUserLogic
    {
        #region Properties

        private Dictionary<Location, TileView> TileMap { get; } = new Dictionary<Location, TileView>();

        private double _scale = 0.10;
        public double Scale
        {
            get { return _scale; }
            set
            {
                if(value < 0.05)
                    value = 0.05;
                if (value > 1.0)
                    value = 1.0;

                _scale = value;
            }
        }

        private double _xOffset = 10.0;
        private double _yOffset = 10.0;
        public double XOffset
        {
            get
            {
                return _xOffset;
            }
            set
            {
                var widthClamp =
                    (Resource.BITMAP_HIT_REFERENCE.PixelWidth * Scale * 0.75 * State.Width) +
                    (Resource.BITMAP_HIT_REFERENCE.PixelWidth * Scale * 0.25) -
                    page.ActualWidth;

                if (value < 0)
                    value = 0;
                else if (value > widthClamp)
                    value = widthClamp;

                _xOffset = value;
            }
        }
        public double YOffset
        {
            get
            {
                return _yOffset;
            }
            set
            {
                var heightClamp = 
                    (Resource.BITMAP_HIT_REFERENCE.PixelHeight * Scale * State.Height) + 
                    (Resource.BITMAP_HIT_REFERENCE.PixelHeight * Scale * 0.5) - 
                    page.ActualHeight;

                if (value < 0)
                    value = 0;
                else if (value > heightClamp)
                    value = heightClamp;

                _yOffset = value;
            }
        }

        public IReadOnlyBattleGameState State
        {
            get
            {
                if (Logic != null)
                    return Logic.State;
                else
                    return null;
            }
        }

        private GameLogic _logic;
        public GameLogic Logic
        {
            get { return _logic; }
            set
            {
                _logic = value;

                if (_logic != null)
                {
                    gameStatePropView.PropertySource = State;
                }
                else
                {
                    gameStatePropView.PropertySource = null;
                }
            }
        }

        public User CurrentUser { get; protected set; }

        private Location lastDownAt;
        private Unit _selectedUnit;
        private Unit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (CanSelectUnit(value) == false)
                    return;

                _selectedUnit = value;
                unitStatePropView.PropertySource = _selectedUnit;
                UpdateHighlights();
            }
        }

        public bool OurTurn { get; protected set; }

        protected Dictionary<Location, List<ActionChain>> SelectedUnitActionMap { get; set; } = new Dictionary<Location, List<ActionChain>>();

        private const int INPUT_TICK_RATE = 50;

        #endregion
        private static readonly bool NETWORKED = Environment.GetCommandLineArgs().Contains("net");
        private static readonly bool SERVER = Environment.GetCommandLineArgs().Contains("server");
        public BattleView()
        {
            InitializeComponent();

            var height = 20;
            var width = 30;

            var terrain = new Terrain[height * width];

            for (var h = 0; h < height; h++)
            {
                for (var w = 0; w < width; w++)
                {
                    var i = h + height * w;
                    terrain[i] = new Terrain(TerrainType.TYPES.Values.First(), new Location(w, h));
                }
            }

            User firstUser = null;
            User secondUser = null;

            if (NETWORKED)
            {
                var view = new UI.NetworkLogViewer();

                //view.Show();

                if (SERVER)
                {
                    var server = new Network.Server(new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), Globals.DEFAULT_PORT));
                    server.Exception += Server_Exception;
                    server.Disconnected += Server_Disconnected;
                    //view.AddSource("Server", server);
                    server.Listen();
                }

                var client = new Network.Client(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), Globals.DEFAULT_PORT));
                client.Exception += Server_Exception;
                client.Disconnected += Server_Disconnected;
                //view.AddSource("Client", client);
                var networked = new NetworkedGameLogic(client, SERVER ? "SERVER" : "CLIENT", new List<IUserLogic>() { this });
                Logic = networked;

                networked.UserSetByServer += Networked_UserSetByServer;
            }
            else
            {
                var local = new LocalGameLogic();
                Logic = local;
                firstUser = new User(0, "First User", true);
                secondUser = new User(1, "Second User", true);
                local.AddUser(firstUser, new List<IUserLogic>() { this });
                local.AddUser(secondUser, new List<IUserLogic>() { this });
                CurrentUser = firstUser;
            }
            

            if (NETWORKED == false || SERVER)
            {
                var unitID = 0;
                Logic.StartGame(
                    new BattleGameState.Fields(height, width, terrain,
                    new Unit[]
                    {
                    new Unit(unitID++, Game.UnitTypes.Infantry.Instance, new Location(6, 6), 0),
                    new Unit(unitID++, Game.UnitTypes.Infantry.Instance, new Location(6, 7), 0),
                    new Unit(unitID++, Game.UnitTypes.Infantry.Instance, new Location(7, 8), 1),
                    new Unit(unitID++, Game.UnitTypes.Infantry.Instance, new Location(5, 8), 1)
                    },
                    new Commander[] { new Commander(0, 0), new Commander(1, 0) }, new Dictionary<string, object>(), -1));
            }

            if (NETWORKED == false)
            {
                Logic.AssignUserToCommander(firstUser.UserID, 0);
                Logic.AssignUserToCommander(secondUser.UserID, 1);
            }

        }

        private void Server_Disconnected(object sender, Network.DisconnectedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ExceptionDispatchInfo.Capture(e.Exception).Throw();
            });
        }

        private void Server_Exception(object sender, Network.ExceptionEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ExceptionDispatchInfo.Capture(e.Exception).Throw();
            });
        }

        private void OnGameLoopException(Exception e)
        {
            Dispatcher.Invoke(() => { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw(); });
        }

        private bool Shutdown = false;
        private async Task GameLoop()
        {
            double interval = 10000 * 50;
            long lastTickAt = DateTime.UtcNow.Ticks;
            TimeSpan waitSpan;
            while(Shutdown == false)
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

        private static readonly double X_SCROLL_MODIFIER = 50.0;
        private static readonly double Y_SCROLL_MODIFIER = X_SCROLL_MODIFIER * (Resource.BITMAP_HIT_REFERENCE.PixelHeight / (double)Resource.BITMAP_HIT_REFERENCE.PixelWidth);

        private void HandleContinuousInput(double modifier)
        {
            var bindings = Settings.Settings.Current.InputBindings;

            var oldYOffset = YOffset;
            var oldXOffset = XOffset;
            if (bindings.MoveUp.Active(activeInput))
            {
                YOffset -= Y_SCROLL_MODIFIER * modifier;
            }
            if(bindings.MoveDown.Active(activeInput))
            {
                YOffset += Y_SCROLL_MODIFIER * modifier;
            }
            if(bindings.MoveLeft.Active(activeInput))
            {
                XOffset -= X_SCROLL_MODIFIER * modifier;
            }
            if(bindings.MoveRight.Active(activeInput))
            {
                XOffset += X_SCROLL_MODIFIER * modifier;
            }

            

            if(oldYOffset != YOffset || oldXOffset != XOffset)
            {
                foreach(var tile in TileMap)
                {
                    tile.Value.UpdatePosition();
                }
            }

            //Window.GetWindow(this).Title = string.Format("{0} , {1} x({2}%)", XOffset, YOffset, Scale);
        }

        private static readonly double ZOOM_MODIFIER = 0.05;
        private void HandleOneTimeInput()
        {
            var bindings = Settings.Settings.Current.InputBindings;

            var oldScale = Scale;
            if(bindings.ZoomIn.Active(activeInput))
            {
                Scale += ZOOM_MODIFIER;
            }
            if(bindings.ZoomOut.Active(activeInput))
            {
                Scale -= ZOOM_MODIFIER;
            }

            if (oldScale != Scale)
            {
                foreach (var tile in TileMap)
                {
                    tile.Value.UpdateScale();
                }
            }
        }

        private void InitTiles()
        {
            canvas.Children.Clear();
            foreach(var kp in TileMap)
            {
                kp.Value.TileViewMouseDown -= View_MouseDown;
                kp.Value.TileViewMouseUp -= View_MouseUp;
            }

            TileMap.Clear();
            for(var h = 0; h < State.Height; h++)
            {
                for(var w = 0; w < State.Width; w++)
                {
                    var view = new TileView(this, new Location(w, h));
                    view.TileViewMouseDown += View_MouseDown;
                    view.TileViewMouseUp += View_MouseUp;
                    canvas.Children.Add(view);
                    TileMap.Add(view.Location, view);
                }
            }
        }

        private void UpdateTurnInfo()
        {
            var currentCommander = State.CurrentCommander;

            OurTurn = currentCommander != null && CurrentUser != null && Logic.CommanderAssignments[currentCommander.CommanderID] == CurrentUser.UserID;
            
            UpdateHighlights();
        }

        private void UpdateHighlights()
        {
            UpdateSelectedUnitActionMap();

            var highlightTargets = SelectedUnitActionMap.Keys;

            foreach (var v in TileMap)
            {
                v.Value.Highlight = highlightTargets.Contains(v.Value.Location);
            }
        }

        private void UpdateSelectedUnitActionMap()
        {
            SelectedUnitActionMap.Clear();

            if (OurTurn == false || SelectedUnit == null)
                return;

            foreach(var action in SelectedUnit.UnitType.Actions.Where(a => a.AvailableDuringTurn))
            {
                foreach(var kp in action.ActionableLocations
                    (
                        State, 
                        new Game.Action.ActionContext(State.CurrentCommander.CommanderID, ActionType.ActionTriggers.None), 
                        State.GetTile(SelectedUnit.Location)
                    )
                )
                {
                    var location = kp.Key;

                    List<ActionChain> listing;
                    if(SelectedUnitActionMap.TryGetValue(location, out listing) == false)
                    {
                        listing = new List<ActionChain>();
                        SelectedUnitActionMap.Add(location, listing);
                    }

                    Contract.Assert(listing != null);

                    listing.Add(kp.Value);
                }
            }
        }

        private bool CanSelectUnit(Unit unit)
        {
            return unit == null || (unit != null && unit.CommanderID == State.CurrentCommander.CommanderID);
        }

        private List<ActionChain> GetAvailableActions(Location location)
        {
            Contract.Ensures(Contract.Result<List<ActionChain>>() != null);

            List<ActionChain> actionChain;

            if (SelectedUnitActionMap.TryGetValue(location, out actionChain))
                return actionChain;
            else
                return new List<ActionChain>(0);
        }

        private Location GetLocationUnderMouse()
        {
            Point p = Mouse.GetPosition(canvas);

            var elm = canvas.InputHitTest(p);

            var view = TileView.GetTileView(elm);

            if (null != view)
                return view.Location;
            else
                return null;
        }

        #region Event Handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += Page_KeyDown;
            window.KeyUp += Page_KeyUp;
            window.MouseDown += Page_MouseDown;
            window.MouseUp += Page_MouseUp;

            GameLoop().ContinueWith(t => { OnGameLoopException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void View_MouseUp(object sender, TileView.TileViewEventArgs e)
        {
            var location = (sender as TileView).Location;

        }

        private void View_MouseDown(object sender, TileView.TileViewEventArgs e)
        {
            if (e.Args.ChangedButton == MouseButton.Left)
            {
                var location = (sender as TileView).Location;

                var unit = State.GetUnit(location);

                if (unit != null && CanSelectUnit(unit))
                {
                    SelectedUnit = unit;
                }
                else if (SelectedUnit != null)
                {
                    List<ActionChain> availableActions = GetAvailableActions(location);
                    if (availableActions.Count > 0)
                    {
                        Logic.DoActions(availableActions.First().GetActionsInfo(State.CurrentCommander.CommanderID, ActionType.ActionTriggers.None));
                    }
                }


                lastDownAt = location;
            }
        }

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
            // TODO Update commander data
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            UpdateUnitAtLocation(e.ChangeInfo.Location);

            if(e.ChangeInfo.LocationChanged)
            {
                UpdateUnitAtLocation(e.ChangeInfo.PreviousLocation);
            }

            UpdateHighlights();
        }

        private void UpdateUnitAtLocation(Location location)
        {
            TileView view;
            if(TileMap.TryGetValue(location, out view))
            {
                view.UpdateUnit();

                if(SelectedUnit != null && SelectedUnit.Location == location)
                {
                    unitStatePropView.Update();
                }
            }
            else
            {
                throw new Game.Exceptions.StateMismatchException();
            }
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            var location = e.ChangeInfo.ChangedTerrainLocation;
            TileView view;
            if(TileMap.TryGetValue(location, out view))
            {
                view.UpdateTerrain();
            }
            else
            {
                throw new Game.Exceptions.StateMismatchException();
            }
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            // TODO Reload/update any state information.
            gameStatePropView.Update();
            UpdateTurnInfo();
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            // TODO
            gameStatePropView.Update();
            UpdateTurnInfo();
            InitTiles();
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            // TODO Update user list
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            // TODO Update user list
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            // TODO Update user list
            UpdateTurnInfo();
        }

        public void OnTurnEnded(object sender, TurnEndedEventArgs e)
        {
            // TODO Figure out if it's currently our turn and enable/disable controls as needed.
            gameStatePropView.Update();
            UpdateTurnInfo();
        }

        public void OnSync(object sender, SyncEventArgs e)
        {
            if (e.SyncID == Logic.LastSyncID)
            {
                gameStatePropView.Update();
                UpdateTurnInfo();
                InitTiles();
            }
        }

        private IList<Settings.Input> activeInput = new List<Settings.Input>();

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            var keyInput = new Settings.KeyInput(e.Key);

            if (activeInput.Contains(keyInput) == false)
            {
                activeInput.Add(keyInput);
                HandleOneTimeInput();
            }
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseInput = new Settings.MouseInput(e.ChangedButton);

            if (activeInput.Contains(mouseInput) == false)
            {
                activeInput.Add(mouseInput);
                HandleOneTimeInput();
            }
        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            activeInput.Remove(new Settings.KeyInput(e.Key));
        }

        private void Page_MouseUp(object sender, MouseButtonEventArgs e)
        {
            activeInput.Remove(new Settings.MouseInput(e.ChangedButton));
        }

        private void endTurnButton_Click(object sender, RoutedEventArgs e)
        {
            Logic.EndTurn(State.CurrentCommander.CommanderID);
        }

        private void Networked_UserSetByServer(object sender, NetworkedGameLogic.UserSetEventArgs e)
        {
            CurrentUser = e.User;
            UpdateHighlights();
            if(SERVER)
            {
                Logic.AssignUserToCommander(CurrentUser.UserID, 0);
            }
            else
            {
                Logic.AssignUserToCommander(CurrentUser.UserID, 1);
            }
        }

        private void canvasContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var location = GetLocationUnderMouse();

            if (null == location)
            {
                e.Handled = true;
                return;
            }

            UpdateCanvasContextMenu(location);

            if (canvasContextMenu.HasItems == false)
            {
                e.Handled = true;
                return;
            }
        }

        private void UpdateCanvasContextMenu(Location location)
        {
            canvasContextMenu.Items.Clear();

            if (location == null)
                return;

            var unit = State.GetUnit(location);

            var availableActions = GetAvailableActions(location);
            if (SelectedUnit != null && availableActions.Count > 0)
            {
                canvasContextMenu.Items.Add(new MenuItem() { IsEnabled = false, Header = "Available Actions", FontWeight = FontWeights.Bold });

                foreach (var action in availableActions)
                {
                    var actionItem = new MenuItem();
                    actionItem.Header = action.GetActions().Aggregate("", (seq, next) => 
                        {
                            if (seq.Length > 0)
                                return seq + " > " + next;
                            else
                                return next.ToString();
                        }).Replace("_", "__");
                    actionItem.Tag = action;
                    actionItem.PreviewMouseDown += CanvasContextMenu_ActionItem_MouseDown;

                    canvasContextMenu.Items.Add(actionItem);
                }
            }

            if (unit != null && CanSelectUnit(unit))
            {
                if (canvasContextMenu.Items.Count > 0)
                    canvasContextMenu.Items.Add(new Separator());
                var selectItem = new MenuItem() { Header = "Select Unit", Tag = location };
                selectItem.PreviewMouseDown += CanvasContextMenu_SelectItem_MouseDown;
                canvasContextMenu.Items.Add(selectItem);
            }
        }

        private void CanvasContextMenu_ActionItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Logic.DoActions(((sender as MenuItem).Tag as ActionChain).GetActionsInfo(State.CurrentCommander.CommanderID, ActionType.ActionTriggers.None));
        }

        private void CanvasContextMenu_SelectItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var unit = State.GetUnit((sender as MenuItem).Tag as Location);

            if (unit != null && CanSelectUnit(unit))
                SelectedUnit = unit;
        }

        #endregion

    }
}
