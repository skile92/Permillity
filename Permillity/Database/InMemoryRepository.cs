using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Database
{
    internal class InMemoryRepository : IRepository
    {
        private readonly List<ApiStats> _stats = new List<ApiStats>();

        public Task<List<ApiStats>> GetAsync()
        {
            return Task.FromResult(_stats.ToList());
        }

        public Task<List<ApiStats>> GetAsync(Guid instance)
        {
            return Task.FromResult(_stats.Where(x => x.Instance == instance).ToList());
        }

        public Task<List<ApiStats>> GetAsync(Guid instance, List<int> requestYears, List<int> requestWeeks, List<string> requestMethods, List<string> requestPaths)
        {
            return Task.FromResult(_stats.Where(x => x.Instance == instance && requestYears.Contains(x.RequestYear) && requestWeeks.Contains(x.RequestWeek) && requestMethods.Contains(x.RequestMethod) && requestPaths.Contains(x.RequestPath)).ToList());
        }

        public Task AddAsync(List<ApiStats> stats)
        {
            _stats.AddRange(stats);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(List<ApiStats> stats)
        {
            foreach (var performance in stats)
            {
                var perf = _stats.First(x => x.Id == performance.Id);

                perf = performance;
            }

            return Task.CompletedTask;
        }
    }
}
