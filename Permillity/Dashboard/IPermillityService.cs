using Permillity.Trackers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Permillity.Dashboard
{
    public interface IPermillityService
    {
        Task<string> GetDashboardAsync();
        Task<List<ApiStats>> GetStatisticsDataAsync();
    }
}
