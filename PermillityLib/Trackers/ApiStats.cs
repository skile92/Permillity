using System;

namespace Permillity.Trackers
{
    public class ApiStats
    {
        public int Id { get; set; }
        public Guid Instance { get; set; }
        public bool IsSuccess { get; set; }
        public int RequestYear { get; set; }
        public int RequestWeek { get; set; }
        public string RequestMethod { get; set; }
        public string RequestPath { get; set; }
        public int RequestCount { get; set; }
        public decimal AverageTime { get; set; }
        public decimal MaximumTime { get; set; }
        public string ExampleQueryString { get; set; }
        public string ExampleBody { get; set; }
    }
}
