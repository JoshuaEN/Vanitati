﻿using System;
using System.Threading;

namespace UnnamedStrategyGame.Network
{
    /// <summary>
    /// Ref: http://stackoverflow.com/a/961904
    /// </summary>
    public sealed class QueuedLock
    {
        private object innerLock;
        private volatile int ticketsCount = 0;
        private volatile int ticketToRide = 1;

        public QueuedLock()
        {
            innerLock = new Object();
        }

        public void Enter()
        {
            int myTicket = Interlocked.Increment(ref ticketsCount);
            Monitor.Enter(innerLock);
            while (true)
            {

                if (myTicket == ticketToRide)
                {
                    return;
                }
                else
                {
                    Monitor.Wait(innerLock);
                }
            }
        }

        public void Exit()
        {
            Interlocked.Increment(ref ticketToRide);
            Monitor.PulseAll(innerLock);
            Monitor.Exit(innerLock);
        }
    }
}
