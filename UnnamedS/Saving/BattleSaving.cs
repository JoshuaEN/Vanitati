using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Saving
{
    public static class BattleSaving
    {
        private static readonly string DefaultPath = Path.Combine(Globals.BaseDataPath, "StateData");
        private static readonly Microsoft.Win32.OpenFileDialog OpenDialog = new Microsoft.Win32.OpenFileDialog() { ValidateNames = true, CheckFileExists = true, CheckPathExists = true, Multiselect = false, DereferenceLinks = true };
        private static readonly Microsoft.Win32.SaveFileDialog SaveDialog = new Microsoft.Win32.SaveFileDialog() { ValidateNames = true, OverwritePrompt = true, AddExtension = true };

        private static void CreatePath()
        {
            Directory.CreateDirectory(DefaultPath);
        }

        public static bool SaveBattleGameState(Wrappers.BaseWrapper wrapper)
        {
            Contract.Requires<ArgumentNullException>(null != wrapper);

            CreatePath();

            SaveDialog.Filter = $"{wrapper.Type.ToString()}|*{wrapper.Extension}";
            SaveDialog.Title = "Save";
            SaveDialog.InitialDirectory = DefaultPath;

            if(SaveDialog.ShowDialog() == true)
            {
                var sText = Serializers.Serializer.Serialize(wrapper);

                File.WriteAllText(SaveDialog.FileName, sText);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static Wrappers.BaseWrapper LoadBattleGameState(List<Wrappers.BaseWrapper> acceptedWrappers)
        {
            Contract.Requires<ArgumentException>(acceptedWrappers == null || acceptedWrappers.Count > 0);

            CreatePath();

            acceptedWrappers = acceptedWrappers ?? Wrappers.BaseWrapper.TYPES.Values.ToList();

            OpenDialog.Filter = string.Join("|", acceptedWrappers.Select(w => $"{w.Type}|*{w.Extension}"));
            OpenDialog.Title = "Open";
            OpenDialog.InitialDirectory = DefaultPath;

            if(OpenDialog.ShowDialog() == true)
            {
                return Serializers.Serializer.Deserialize<Wrappers.BaseWrapper>(File.ReadAllText(OpenDialog.FileName));
            }
            else
            {
                return null;
            }
        }
    }
}
