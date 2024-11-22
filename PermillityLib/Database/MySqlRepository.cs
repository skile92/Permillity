using Dapper;
using MySqlConnector;
using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Database
{
    internal class MySqlRepository : IRepository
    {
        private readonly string _connectionString;

        public MySqlRepository(PermillityOptions options)
        {
            _connectionString = options.ConnectionString ?? "";

            CreatePermillityTables();
        }

        public async Task<List<ApiStats>> GetAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats;";
                var stats = (await connection.QueryAsync<ApiStats>(sql)).ToList();

                return stats;
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats WHERE Instance = @Instance;";
                var stats = (await connection.QueryAsync<ApiStats>(sql, instance)).ToList();

                return stats;
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance, List<int> requestYears, List<int> requestWeeks, List<string> requestMethods, List<string> requestPaths)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats WHERE Instance = @Instance AND RequestYear IN @RequestYears AND RequestWeek IN @RequestWeeks AND RequestMethod IN @RequestMethods" +
                    " AND RequestPath IN @RequestPaths;";

                var parameters = new { Instance = instance, RequestYears = requestYears, RequestWeeks = requestWeeks, RequestMethods = requestMethods, RequestPaths = requestPaths };
                var stats = (await connection.QueryAsync<ApiStats>(sql, parameters)).ToList();

                return stats;
            }                
        }

        public async Task AddAsync(List<ApiStats> stats)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "INSERT INTO ApiStats (Instance, IsSuccess, RequestYear, RequestWeek, RequestMethod, RequestPath, RequestCount, AverageTime, MaximumTime, ExampleQueryString, ExampleBody)" +
                    " VALUES (@Instance, @IsSuccess, @RequestYear, @RequestWeek, @RequestMethod, @RequestPath, @RequestCount, @AverageTime, @MaximumTime, @ExampleQueryString, @ExampleBody)";

                await connection.ExecuteAsync(sql, stats);
            }
        }

        public async Task UpdateAsync(List<ApiStats> stats)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE ApiStats SET RequestWeek = @RequestWeek, RequestYear = @RequestYear, RequestMethod = @RequestMethod, RequestPath = @RequestPath, " +
                    "RequestCount = @RequestCount, AverageTime = @AverageTime, MaximumTime = @MaximumTime, ExampleQueryString = @ExampleQueryString, ExampleBody = @ExampleBody WHERE Id = @Id";

                foreach (var stat in stats)
                {
                    await connection.ExecuteAsync(sql, stat);
                }
            }                
        }

        private void CreatePermillityTables()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var createTableSql = "CREATE TABLE IF NOT EXISTS ApiStats (Id INT AUTO_INCREMENT PRIMARY KEY,Instance CHAR(36) NOT NULL, IsSuccess BOOLEAN NOT NULL, RequestWeek INT NOT NULL," +
                    " RequestYear INT NOT NULL, RequestMethod VARCHAR(10) NOT NULL, RequestPath VARCHAR(500) NOT NULL, RequestCount INT NOT NULL, AverageTime DECIMAL(8, 2) NOT NULL," +
                    " MaximumTime DECIMAL(8, 2) NOT NULL, ExampleQueryString VARCHAR(1000) NULL, ExampleBody VARCHAR(1000) NULL);";
                connection.Execute(createTableSql);
            }
        }
    }
}
