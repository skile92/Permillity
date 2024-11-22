namespace Permillity
{
    public class PermillityOptionsBuilder
    {
        private int _databaseBatchSize = 50;
        private bool _useInMemory = true;
        private bool _useMySql = false;
        private string _connectionString = null;
        private bool _useLogger = false;
        private string[] _ignoreEndpoints = new string[0];

        /// <summary>
        /// Directs permillity to use in-memory database.
        /// </summary>
        /// <param name="useInMemory">Should use in-memory database. Default is true.</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder UseInMemory(bool useInMemory = true)
        {
            _useInMemory = useInMemory;
            _useMySql = false;

            return this;
        }

        /// <summary>
        /// Directs permillity to use ms sql server database.
        /// </summary>
        /// <param name="connectionString">Connection string to ms sql server database.</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder UseSqlServer(string connectionString)
        {
            _useInMemory = false;
            _useMySql = false;

            _connectionString = connectionString;

            return this;
        }

        /// <summary>
        /// Directs permillity to use my sql database.
        /// </summary>
        /// <param name="connectionString">Connection string to my sql database.</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder UseMySql(string connectionString)
        {
            _useInMemory = false;
            _useMySql = true;
            _connectionString = connectionString;

            return this;
        }

        /// <summary>
        /// Directs permillity to use logger if available. It will log erros if they happen. ILogger should be registered separately.
        /// </summary>
        /// <param name="useLogger">Should use logger. Default is true.</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder UseLogger(bool useLogger = true)
        {
            _useLogger = useLogger;

            return this;
        }

        /// <summary>
        /// Specifies number of requests that should pass before writing statistics to database. For better performance use higher number.
        /// For more up-to-date statistics use lower number. For writing statistics on every request (real-time) use 0. Default is 50. 
        /// </summary>
        /// <param name="batchSize">Specifies batch size. Default is 50.</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder SetBatchSize(int batchSize)
        {
            _databaseBatchSize = batchSize;

            return this;
        }

        /// <summary>
        /// Sets list of endpoints that are ignored. Endpoints should be passed as route templates, for example api/GetWeatherForecast or api/GetWeatherForecast/{id}.
        /// </summary>
        /// <param name="ignoreEndpoints">List of endpoints in route template format (for example: api/GetWeatherForecast or api/GetWeatherForecast/{id}).</param>
        /// <returns>Options builder</returns>
        public PermillityOptionsBuilder SetIgnoreEndpoints(string[] ignoreEndpoints)
        {
            _ignoreEndpoints = ignoreEndpoints;

            return this;
        }

        /// <summary>
        /// Builds permillity options object.
        /// </summary>
        /// <returns>Options</returns>
        public PermillityOptions Build()
        {
            return new PermillityOptions(
                _useInMemory,
                _useMySql,
                _connectionString,
                _databaseBatchSize,
                _useLogger,
                _ignoreEndpoints
            );
        }
    }
}
