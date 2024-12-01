using System;

namespace Permillity
{
    public class PermillityOptions
    {
        public int DatabaseBatchSize { get; set; }
        public bool UseInMemory { get; set; }
        public bool UseMySql { get; set; }
        public string ConnectionString { get; set; }
        public bool UseLogger { get; set; }
        public string[] IgnoreEndpoints { get; set; }

        /// <summary>
        /// Options for configuring permillity services.
        /// </summary>
        /// <param name="useInMemory">Should use in memory storage. If passed true, connection string will not be taken into account.</param>
        /// <param name="useMySql">Should use mysql database. If false, and in memory is false, ms sql server will be used.</param>
        /// <param name="connectionString">Connection string to database. If useInMemory is true this parameter should be null.</param>
        /// <param name="databaseBatchSize">Specifies number of requests that should pass before writing statistics to database. For better performance use higher number.
        /// For more up-to-date statistics use lower number. For writing statistics on every request (real-time) use 0. Default is 50.</param>
        /// <param name="useLogger">Should permillity log error if happens. ILogger should be registered.</param>
        /// <param name="ignoreEndpoints">List of endpoints that are ignored. Endpoints should be passed as route templates, for example api/GetWeatherForecast or api/GetWeatherForecast/{id}.</param>
        public PermillityOptions(bool useInMemory = true, bool useMySql = false, string connectionString = null, int databaseBatchSize = 50, bool useLogger = false, string[] ignoreEndpoints = null)
        {
            UseInMemory = useInMemory;
            UseMySql = useMySql;
            ConnectionString = connectionString;
            DatabaseBatchSize = databaseBatchSize;
            UseLogger = useLogger;
            IgnoreEndpoints = ignoreEndpoints ?? Array.Empty<string>();
        }
    }
}
