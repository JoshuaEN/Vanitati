﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnGameStateChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.GameStateChangedArgs Args { get; }

        public OnGameStateChangedNotifyWrapper(Game.Event.GameStateChangedArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnGameStateChanged(this, Args);
        }
    }
}
