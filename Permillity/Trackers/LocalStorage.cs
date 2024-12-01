using System.Collections.Concurrent;

namespace Permillity.Trackers
{
    internal class LocalStorage
    {
        public readonly ConcurrentBag<CustomRequest> Requests;

        public LocalStorage()
        {
            Requests = new ConcurrentBag<CustomRequest>();
        }

        public void Clear()
        {
            while (Requests.TryTake(out _)) { }
        }
    }
}
