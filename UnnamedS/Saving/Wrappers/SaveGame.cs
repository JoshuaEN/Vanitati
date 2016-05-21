using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Saving.Wrappers
{
    public class SaveGame : BaseWrapper
    {
        public GameMode Mode { get; }
        public override SaveType Type { get; } = SaveType.SaveGame;
        public override string Extension { get; } = ".VanitatiSave";

        public SaveGame(Game.BattleGameState.Fields fields, GameMode mode) : base(fields)
        {
            Mode = mode;
        }

        public enum GameMode { Local, Networked };

        private SaveGame() : base() { }
        public static SaveGame Instance { get; } = new SaveGame();
    }
}
