using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        static Resource()
        {
            var sprites = new Dictionary<string, BitmapImage>();

            LoadSpritesFor(
                UnitType.TYPES.ToDictionary(k => k.Key, k => (BaseType)k.Value), 
                new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "unit_missing.png"), UriKind.Relative)), 
                ref sprites
            );

            LoadSpritesFor(
                TerrainType.TYPES.ToDictionary(k => k.Key, k => (BaseType)k.Value),
                new BitmapImage(new Uri(Path.Combine(Globals.RESOURCE_IMAGE_PATH, "terrain_missing.png"), UriKind.Relative)),
                ref sprites
            );

            SPRITES = sprites;
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
    }
}
