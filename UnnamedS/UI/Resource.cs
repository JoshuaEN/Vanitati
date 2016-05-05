using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.UI
{
    public static class Resource
    {
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
    }
}
