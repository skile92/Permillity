using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Permillity.Database
{
    internal interface IRepository
    {
        Task<List<ApiStats>> GetAsync();
        Task<List<ApiStats>> GetAsync(Guid instance);
        Task<List<ApiStats>> GetAsync(Guid instance, List<int> requestYears, List<int> requestWeeks, List<string> requestMethods, List<string> requestPaths);
        Task AddAsync(List<ApiStats> stats);
        Task UpdateAsync(List<ApiStats> stats);
    }
}
