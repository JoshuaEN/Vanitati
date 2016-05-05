using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;
using Xunit;

namespace UnnamedStrategyGameTests.TestHelpers.FakeConcrete
{
    public class FakeIUserLogic : IUserLogic
    {
        public enum Handler
        {
            OnActionsTaken,
            OnCommanderChanged,
            OnGameStart,
            OnGameStateChanged,
            OnSync,
            OnTerrainChanged,
            OnTurnChanged,
            OnUnitChanged,
            OnUserAdded,
            OnUserAssignedToCommander,
            OnUserRemoved,
            OnAny,
            EndOfHandlers
        };

        private bool endOfHandlersFlag = false;

        public bool Pause { get; set; }

        public struct Callback
        {
            public Handler Handler;
            public Action<EventArgs> Action;

            public Callback(Handler handler, Action<EventArgs> action)
            {
                Handler = handler;
                Action = action;
            }
        }

        public Queue<Callback> CallbackOrder { get; }

        public FakeIUserLogic(params Callback[] callbacks)
        {
            CallbackOrder = new Queue<Callback>(callbacks);
        }

        public FakeIUserLogic AddCallbacks(params Callback[] callbacks)
        {
            foreach (var callback in callbacks)
                CallbackOrder.Enqueue(callback);

            return this;
        }

        public void OnActionsTaken(object sender, ActionsTakenEventArgs e)
        {
            On(Handler.OnActionsTaken, e);
        }

        public void OnCommanderChanged(object sender, CommanderChangedEventArgs e)
        {
            On(Handler.OnCommanderChanged, e);
        }

        public void OnGameStart(object sender, GameStartEventArgs e)
        {
            On(Handler.OnGameStart, e);
        }

        public void OnGameStateChanged(object sender, GameStateChangedArgs e)
        {
            On(Handler.OnGameStateChanged, e);
        }

        public void OnSync(object sender, SyncEventArgs args)
        {
            On(Handler.OnSync, args);
        }

        public void OnTerrainChanged(object sender, TerrainChangedEventArgs e)
        {
            On(Handler.OnTerrainChanged, e);
        }

        public void OnTurnChanged(object sender, TurnChangedEventArgs args)
        {
            On(Handler.OnTurnChanged, args);
        }

        public void OnUnitChanged(object sender, UnitChangedEventArgs e)
        {
            On(Handler.OnUnitChanged, e);
        }

        public void OnUserAdded(object sender, UserAddedEventArgs e)
        {
            On(Handler.OnUserAdded, e);
        }

        public void OnUserAssignedToCommander(object sender, UserAssignedToCommanderEventArgs e)
        {
            On(Handler.OnUserAssignedToCommander, e);
        }

        public void OnUserRemoved(object sender, UserRemovedEventArgs e)
        {
            On(Handler.OnUserRemoved, e);
        }

        private void On(Handler handler, EventArgs e)
        {
            if (endOfHandlersFlag || Pause)
                return;

            var callback = CallbackOrder.Dequeue();

            if (callback.Handler == handler || callback.Handler == Handler.OnAny)
                callback.Action?.Invoke(e);
            else if (callback.Handler == Handler.EndOfHandlers)
                endOfHandlersFlag = true;
            else
                Assert.True(false, $"Expected callback for {callback.Handler}, got {handler}");
        }

        public void End()
        {
            Assert.Empty(CallbackOrder);
        }
    }
}
