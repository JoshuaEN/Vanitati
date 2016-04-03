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
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.UI
{
    /// <summary>
    /// Interaction logic for BattleView.xaml
    /// </summary>
    public partial class BattleView : Page, IPlayerLogic
    {
        private Dictionary<Location, TileView> TileMap { get; } = new Dictionary<Location, TileView>();
        public double Scale { get; set; } = 0.20;
        public BattleGameState GameState { get; set; }
        public GameLogic Logic { get; set; }

        public BattleView()
        {
            InitializeComponent();

            var height = 50;
            var width = 100;

            var terrain = new Terrain[height * width];

            for(var i = 0; i < height * width; i++)
            {
                terrain[i] = new Terrain(TerrainType.TYPES.Keys.First());
            }

            GameState = new Game.BattleGameState();
            var local = new LocalGameLogic(GameState);
            Logic = local;
            Logic.StartGame(height, width, terrain, new Unit[0], new Player[0], new Dictionary<string, object>());

            var firstPlayerID = local.AddPlayerLocally(new List<IPlayerLogic>() { this });
            var secondPlayerID = local.AddPlayerLocally(new List<IPlayerLogic>() { this });

            

            InitTiles();

            //var bitmap = new BitmapImage(new Uri(System.IO.Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_road.png"), UriKind.Relative));
            //var imgSource = new DrawingImage(new ImageDrawing(bitmap, new Rect(0, 0, bitmap.Width, bitmap.Height)));

            //var img = new Image();
            //img.Source = imgSource;
            //img.Height = bitmap.Height;
            //img.Width = bitmap.Width;
            //img.Stretch = Stretch.None;
            //img.IsHitTestVisible = false;
            
            //canvas.Children.Add(img);
        }

        private void InitTiles()
        {
            canvas.Children.Clear();
            for(var h = 0; h <= GameState.Height; h++)
            {
                for(var w = 0; w <= GameState.Width; w++)
                {
                    canvas.Children.Add(
                        new TileView(this, new Location(w, h))
                    );
                }
            }
        }

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerChanged(object sender, PlayerChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            e.ChangeInfo.
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnThisPlayerAdded(object sender, ThisPlayerAddedArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
