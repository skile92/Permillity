namespace Permillity.Dashboard
{
    public class DashboardStatistics
    {
        public string RequestMethod { get; set; }
        public string RequestPath { get; set; }
        public int RequestCount { get; set; }
        public decimal AverageTime { get; set; }
        public decimal MaximumTime { get; set; }
        public decimal TotalTime { get; set; }
        public string ExampleQueryString { get; set; }
        public string ExampleBody { get; set; }

        public int SuccessRequestCount { get; set; }
        public decimal SuccessAverageTime { get; set; }
        public decimal SuccessMaximumTime { get; set; }
        public decimal SuccessTotalTime { get; set; }

        public int FailureRequestCount { get; set; }
        public decimal FailureAverageTime { get; set; }
        public decimal FailureMaximumTime { get; set; }
        public decimal FailureTotalTime { get; set; }

        public int ThisWeekRequestCount { get; set; }
        public decimal ThisWeekAverageTime { get; set; }
        public decimal ThisWeekMaximumTime { get; set; }
        public decimal ThisWeekTotalTime { get; set; }
    }
}
