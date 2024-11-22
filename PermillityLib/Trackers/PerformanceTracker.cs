using Microsoft.Extensions.Logging;
using Permillity.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Trackers
{
    internal class PerformanceTracker : IPerformanceTracker
    {
        private readonly LocalStorage _storage;
        private readonly IRepository _repository;
        private readonly ILogger<PerformanceTracker> _logger;
        private readonly Coordinator _coordinator;
        private readonly int _batchSize;
        private readonly bool _useLogger;

        public PerformanceTracker(IRepository repository, PermillityOptions options, ILogger<PerformanceTracker> logger, Coordinator coordinator, LocalStorage storage)
        {
            _storage = storage;
            _repository = repository;
            _logger = logger;
            _batchSize = options.DatabaseBatchSize;
            _useLogger = options.UseLogger;
            _coordinator = coordinator;
        }

        public async Task ProcessAsync(string method, string path, string query, string body, decimal time, bool isSuccess)
        {
            var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            var year = DateTime.Now.Year;

            var performance = new CustomRequest()
            {
                Year = year,
                Week = week,
                ExecutionTime = time,
                Method = method,
                Path = path,
                Body = body,
                QueryString = query,
                IsSuccess = isSuccess
            };
            _storage.Requests.Add(performance);

            if (_storage.Requests.Count < _batchSize)
                return;

            await _coordinator.Semaphore.WaitAsync();

            try
            {
                await WriteToRepository();

                _storage.Clear();
            }
            catch (Exception ex)
            {
                if (_useLogger)
                    _logger?.LogError(ex, "Permillity error.");
            }
            finally
            {
                _coordinator.Semaphore.Release();
            }
        }

        private async Task WriteToRepository()
        {
            var groupedRequests = _storage.Requests
                .GroupBy(x => new { x.Year, x.Week, x.Path, x.Method, x.IsSuccess })
                .Select(x => new ApiStats
                {
                    Instance = _coordinator.Instance,
                    IsSuccess = x.Key.IsSuccess,
                    RequestYear = x.Key.Year,
                    RequestWeek = x.Key.Week,
                    RequestPath = x.Key.Path,
                    RequestMethod = x.Key.Method,
                    RequestCount = x.Count(),
                    AverageTime = x.Average(y => y.ExecutionTime),
                    MaximumTime = x.Max(y => y.ExecutionTime),
                    ExampleQueryString = x.OrderByDescending(y => y.ExecutionTime)?.FirstOrDefault()?.QueryString,
                    ExampleBody = x.OrderByDescending(y => y.ExecutionTime)?.FirstOrDefault()?.Body
                })
                .ToList();

            var weeks = _storage.Requests.Select(x => x.Week).Distinct().ToList();
            var years = _storage.Requests.Select(x => x.Year).Distinct().ToList();
            var methods = _storage.Requests.Select(x => x.Method).Distinct().ToList();
            var paths = _storage.Requests.Select(x => x.Path).Distinct().ToList();

            var repoStatistics = await _repository.GetAsync(_coordinator.Instance, years, weeks, methods, paths);

            var itemsToAdd = new List<ApiStats>();
            var itemsToUpdate = new List<ApiStats>();
            foreach (var request in groupedRequests)
            {
                var repoRequest = repoStatistics
                                    .FirstOrDefault(x => x.IsSuccess == request.IsSuccess && x.RequestWeek == request.RequestWeek && x.RequestYear == request.RequestYear
                                                      && x.RequestMethod == request.RequestMethod && x.RequestPath == request.RequestPath);

                if (repoRequest == null)
                {
                    itemsToAdd.Add(request);
                    continue;
                }

                repoRequest.RequestCount += request.RequestCount;
                repoRequest.AverageTime = (repoRequest.AverageTime + request.AverageTime) / 2;

                if (request.MaximumTime > repoRequest.MaximumTime)
                {
                    repoRequest.MaximumTime = request.MaximumTime;
                    repoRequest.ExampleBody = request.ExampleBody;
                    repoRequest.ExampleQueryString = request.ExampleQueryString;
                }

                itemsToUpdate.Add(repoRequest);
            }

            await _repository.AddAsync(itemsToAdd);
            await _repository.UpdateAsync(itemsToUpdate);
        }
    }
}
