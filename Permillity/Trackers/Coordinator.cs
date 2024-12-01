using System;
using System.Threading;

namespace Permillity.Trackers
{
    internal class Coordinator
    {
        public readonly SemaphoreSlim Semaphore;
        public readonly Guid Instance;

        public Coordinator()
        {
            Semaphore = new SemaphoreSlim(1,1);
            Instance = Guid.NewGuid();
        }
    }
}
