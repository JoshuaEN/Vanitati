using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI
{
    public static class Resource
    {

        public static MainWindow MainWindow { get; set; }


        public static readonly Brush NPC_BRUSH = Brushes.LightGray;
        public static readonly Brush CANNOT_BE_CAPTURED_BRUSH = Brushes.Transparent;

        public static readonly IReadOnlyDictionary<string, BitmapImage> SPRITES;

        public static readonly BitmapImage BITMAP_HIT_REFERENCE =
            new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_hit_reference.png"), UriKind.Relative));

        public static readonly BitmapImage BITMAP_NO_UNIT =
            new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "unit_none.png"), UriKind.Relative));

        public static readonly IReadOnlyDictionary<string, Brush> BRUSHES;

        static Resource()
        {
            var sprites = new Dictionary<string, BitmapImage>();
            var brushes = new Dictionary<string, Brush>();

            LoadDataFor(
                UnitType.TYPES.ToDictionary(k => k.Key, k => (BaseType)k.Value), 
                new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "unit_missing.png"), UriKind.Relative)), 
                ref sprites,
                ref brushes
            );

            LoadDataFor(
                TerrainType.TYPES.ToDictionary(k => k.Key, k => (BaseType)k.Value),
                new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_missing.png"), UriKind.Relative)),
                ref sprites,
                ref brushes
            );

            SPRITES = sprites;
            BRUSHES = brushes;
        }

        private static void LoadDataFor(Dictionary<string, BaseType> types, BitmapImage missingImage, ref Dictionary<string, BitmapImage> sprites, ref Dictionary<string, Brush> brushes)
        {
            LoadSpritesFor(types, missingImage, ref sprites);
            FillMissingBrushes(types, ref brushes);
        }

        private static void LoadSpritesFor(Dictionary<string, BaseType> types, BitmapImage missingImage, ref Dictionary<string, BitmapImage> sprites)
        {
            foreach (var type in types)
            {
                BitmapImage image;

                var path = Path.Combine(Globals.RESOURCE_IMAGE_PATH, type.Key + ".png");
                if (File.Exists(path) == false)
                {
                    image = missingImage;
                }
                else
                {
                    var uri = new Uri(path, UriKind.Relative);
                    image = new BitmapImage(uri);
                }

                sprites.Add(type.Key, image);
            }
        }

        private static void FillMissingBrushes(Dictionary<string, BaseType> types, ref Dictionary<string, Brush> brushes)
        {
            foreach(var type in types)
            {
                if (brushes.ContainsKey(type.Key))
                    continue;

                var keyColors = Encoding.Default.GetBytes(type.Key.Split(new char[] { '_'}, 2)[1].ToCharArray());

                var color = Colors.Purple;

                if (keyColors.Length > 2)
                    color = Color.FromArgb(255, keyColors[0], keyColors[1], keyColors[2]);

                brushes.Add(type.Key, new SolidColorBrush(color));
            }
        }

        public static void GetCommanderIDNameAndColorForTerrain(GameLogic logic, Terrain terrain, out string text, out Brush background)
        {

            if (terrain.TerrainType.CanCapture == false)
            {
                text = "Cannot be Captured";
                background = CANNOT_BE_CAPTURED_BRUSH;
            }
            else if (terrain.IsOwned == true)
            {
                GetCommanderIDNameAndColor(logic, terrain.CommanderID, out text, out background);
            }
            else
            {
                text = "Unowned";
                background = NPC_BRUSH;
            }
        }

        public static void GetCommanderIDNameAndColor(GameLogic logic, int commanderID, out string text, out Brush background)
        {
            int? userID;
            User user;

            if (logic.CommanderAssignments.TryGetValue(commanderID, out userID) == false)
            {
                text = "INVALID_COMMANDER_ID";
                background = Brushes.Red;
            }
            else if (userID == null)
            {
                text = "UNASSIGNED_COMMANDER";
                background = Globals.GetCommanderColor(commanderID);
            }
            else if (logic.Users.TryGetValue(userID.Value, out user) == false)
            {
                text = "INVALID_USER_LINKAGE";
                background = Globals.GetCommanderColor(commanderID);
            }
            else
            {
                text = user.Name;
                background = Globals.GetCommanderColor(commanderID);
            }
        }

        public static Brush GetCommanderColor(GameLogic logic, int commanderID)
        {
            string text;
            Brush background;
            GetCommanderIDNameAndColor(logic, commanderID, out text, out background);
            return background;
        }

        public static string GetCommanderText(GameLogic logic, int commanderID)
        {
            string text;
            Brush background;
            GetCommanderIDNameAndColor(logic, commanderID, out text, out background);
            return text;
        }

        public static void GenerateRow(Grid grid, ref int rowNumber, params object[] columns)
        {
            grid.RowDefinitions.Add(new RowDefinition());

            for(var i = 0; i < columns.Length; i++)
            {
                var obj = columns[i];

                var elm = (obj as System.Windows.UIElement) ?? new Label() { Content = obj };

                Grid.SetColumn(elm, i);
                Grid.SetRow(elm, rowNumber);

                grid.Children.Add(elm);
            }

            rowNumber += 1;
        }

        public static bool ShowValidationErrors(List<string> errors)
        {
            
            if (errors.Count > 0)
            {
                System.Windows.MessageBox.Show($"There were {errors.Count} error(s):\n\t{string.Join("\n\t", errors)}\n\nPlease Address these issues to continue.", $"{errors.Count} Error(s)", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
