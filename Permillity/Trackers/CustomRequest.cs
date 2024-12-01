namespace Permillity.Trackers
{
    internal class CustomRequest
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public bool IsSuccess { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Body { get; set; }
        public decimal ExecutionTime { get; set; }
    }
}
