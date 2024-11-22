using Newtonsoft.Json;
using Permillity.Database;
using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Dashboard
{
    internal class PermillityService : IPermillityService
    {
        private readonly IRepository _repository;

        public PermillityService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ApiStats>> GetStatisticsDataAsync()
        {
            var data = await _repository.GetAsync();

            return data;
        }

        public async Task<string> GetDashboardAsync()
        {
            var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            var apiStats = await _repository.GetAsync();

            var data = ConvertToDashStats(apiStats, week);

            var serializedData = JsonConvert.SerializeObject(data);

            return HtmlBuilder.GetDashboardHtml(serializedData);
        }

        private List<DashboardStatistics> ConvertToDashStats(List<ApiStats> apiStats, int week)
        {
            var dataGrouping = apiStats
                .GroupBy(x => new { x.RequestMethod, x.RequestPath });

            var data = new List<DashboardStatistics>();

            foreach (var item in dataGrouping)
            {
                var successes = item.Where(x => x.IsSuccess).ToList();
                var failures = item.Where(x => !x.IsSuccess).ToList();
                var ofThisWeek = item.Where(x => x.RequestWeek == week).ToList();

                var dashStat = new DashboardStatistics()
                {
                    RequestPath = item.Key.RequestPath,
                    RequestMethod = item.Key.RequestMethod,
                    ExampleBody = item.OrderByDescending(y => y.MaximumTime)?.FirstOrDefault()?.ExampleBody,
                    ExampleQueryString = item.OrderByDescending(y => y.MaximumTime)?.FirstOrDefault().ExampleQueryString,

                    RequestCount = item.Sum(y => y.RequestCount),
                    AverageTime = item.Sum(y => y.AverageTime) / item.Count(),
                    MaximumTime = item.Max(y => y.MaximumTime),
                    TotalTime = item.Sum(y => y.RequestCount) * (item.Sum(y => y.AverageTime) / (1000 * item.Count())),

                    SuccessRequestCount = successes.Any() ? successes.Sum(y => y.RequestCount) : 0,
                    SuccessAverageTime = successes.Any() ? successes.Sum(y => y.AverageTime) / successes.Count() : 0,
                    SuccessMaximumTime = successes.Any() ? successes.Max(y => y.MaximumTime) : 0,
                    SuccessTotalTime = successes.Any() ? successes.Sum(y => y.RequestCount) * (successes.Sum(y => y.AverageTime) / (1000 * successes.Count())) : 0,

                    FailureRequestCount = failures.Any() ? failures.Sum(y => y.RequestCount) : 0,
                    FailureAverageTime = failures.Any() ? failures.Sum(y => y.AverageTime) / failures.Count() : 0,
                    FailureMaximumTime = failures.Any() ? failures.Max(y => y.MaximumTime) : 0,
                    FailureTotalTime = failures.Any() ? failures.Sum(y => y.RequestCount) * (failures.Sum(y => y.AverageTime) / (1000 * failures.Count())) : 0,

                    ThisWeekRequestCount = ofThisWeek.Any() ? ofThisWeek.Sum(y => y.RequestCount) : 0,
                    ThisWeekAverageTime = ofThisWeek.Any() ? ofThisWeek.Sum(y => y.AverageTime) / ofThisWeek.Count() : 0,
                    ThisWeekMaximumTime = ofThisWeek.Any() ? ofThisWeek.Max(y => y.MaximumTime) : 0,
                    ThisWeekTotalTime = ofThisWeek.Any() ? ofThisWeek.Sum(y => y.RequestCount) * (ofThisWeek.Sum(y => y.AverageTime) / (1000 * ofThisWeek.Count())) : 0
                };

                //round decimals
                dashStat.AverageTime = Math.Round(dashStat.AverageTime, 4);
                dashStat.TotalTime = Math.Round(dashStat.TotalTime, 4);
                dashStat.SuccessAverageTime = Math.Round(dashStat.SuccessAverageTime, 4);
                dashStat.SuccessTotalTime = Math.Round(dashStat.SuccessTotalTime, 4);
                dashStat.FailureAverageTime = Math.Round(dashStat.FailureAverageTime, 4);
                dashStat.FailureTotalTime = Math.Round(dashStat.FailureTotalTime, 4);
                dashStat.ThisWeekAverageTime = Math.Round(dashStat.ThisWeekAverageTime, 4);
                dashStat.ThisWeekTotalTime = Math.Round(dashStat.ThisWeekTotalTime, 4);

                data.Add(dashStat);
            }

            return data;
        }
    }
}
