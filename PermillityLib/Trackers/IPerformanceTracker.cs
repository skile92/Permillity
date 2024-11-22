using System.Threading.Tasks;

namespace Permillity.Trackers
{
    internal interface IPerformanceTracker
    {
        Task ProcessAsync(string method, string path, string query, string body, decimal time, bool isSuccess);
    }
}
