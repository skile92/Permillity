using Dapper;
using Microsoft.Data.Sqlite;
using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Database
{
    internal class SqliteRepository : IRepository
    {
        private readonly string _connectionString;

        public SqliteRepository(PermillityOptions options)
        {
            _connectionString = options.ConnectionString ?? "";

            CreatePermillityTables();
        }

        public async Task<List<ApiStats>> GetAsync()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats;";

                var stats = (await connection.QueryAsync(sql)).ToList();

                return ConvertRaw(stats);
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats WHERE Instance = @Instance;";
                var stats = (await connection.QueryAsync(sql, instance)).ToList();

                return ConvertRaw(stats);
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance, List<int> requestYears, List<int> requestWeeks, List<string> requestMethods, List<string> requestPaths)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "SELECT * FROM ApiStats WHERE Instance = @Instance AND RequestYear IN @RequestYears AND RequestWeek IN @RequestWeeks AND RequestMethod IN @RequestMethods" +
                    " AND RequestPath IN @RequestPaths;";

                var parameters = new { Instance = instance, RequestYears = requestYears, RequestWeeks = requestWeeks, RequestMethods = requestMethods, RequestPaths = requestPaths };
                var stats = (await connection.QueryAsync(sql, parameters)).ToList();

                return ConvertRaw(stats);
            }
        }

        public async Task AddAsync(List<ApiStats> stats)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sql = "INSERT INTO ApiStats (Instance, IsSuccess, RequestYear, RequestWeek, RequestMethod, RequestPath, RequestCount, AverageTime, MaximumTime, ExampleQueryString, ExampleBody)" +
                    " VALUES (@Instance, @IsSuccess, @RequestYear, @RequestWeek, @RequestMethod, @RequestPath, @RequestCount, @AverageTime, @MaximumTime, @ExampleQueryString, @ExampleBody)";

                await connection.ExecuteAsync(sql, stats);
            }
        }

        public async Task UpdateAsync(List<ApiStats> stats)
        {
            using (var connection = new SqliteConnection(_connectionString))
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
            using (var connection = new SqliteConnection(_connectionString))
            {
                var createTableSql = "CREATE TABLE IF NOT EXISTS ApiStats (Id INTEGER PRIMARY KEY AUTOINCREMENT, Instance TEXT NOT NULL, IsSuccess BOOLEAN NOT NULL, RequestWeek INTEGER NOT NULL," +
                    " RequestYear INTEGER NOT NULL, RequestMethod TEXT NOT NULL, RequestPath TEXT NOT NULL, RequestCount INTEGER NOT NULL, AverageTime REAL NOT NULL, MaximumTime REAL NOT NULL," +
                    " ExampleQueryString TEXT NULL, ExampleBody TEXT NULL); ";
                connection.Execute(createTableSql);
            }
        }

        //must use for conversion of sqlite data types to dotnet
        private List<ApiStats> ConvertRaw(List<dynamic> rawData)
        {
            return rawData.Select(x => new ApiStats
            {
                Id = (int)x.Id,
                Instance = Guid.Parse(x.Instance),
                IsSuccess = Convert.ToBoolean(x.IsSuccess),
                RequestYear = (int)x.RequestYear,
                RequestWeek = (int)x.RequestWeek,
                RequestMethod = x.RequestMethod,
                RequestPath = x.RequestPath,
                RequestCount = (int)x.RequestCount,
                AverageTime = (decimal)x.AverageTime,
                MaximumTime = (decimal)x.MaximumTime,
                ExampleQueryString = x.ExampleQueryString,
                ExampleBody = x.ExampleBody
            }).ToList();
        }
    }
}
