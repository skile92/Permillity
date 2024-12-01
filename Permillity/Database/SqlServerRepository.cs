using Dapper;
using Microsoft.Data.SqlClient;
using Permillity.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Database
{
    internal class SqlServerRepository : IRepository
    {
        private readonly string _connectionString;

        public SqlServerRepository(PermillityOptions options)
        {
            _connectionString = options.ConnectionString ?? "";

            CreatePermillityTables();
        }

        public async Task<List<ApiStats>> GetAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM [Permillity].[ApiStats];";
                var stats = (await connection.QueryAsync<ApiStats>(sql)).ToList();

                return stats;
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM [Permillity].[ApiStats] WHERE [Instance] = @Instance;";
                var stats = (await connection.QueryAsync<ApiStats>(sql, instance)).ToList();

                return stats;
            }
        }

        public async Task<List<ApiStats>> GetAsync(Guid instance, List<int> requestYears, List<int> requestWeeks, List<string> requestMethods, List<string> requestPaths)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM [Permillity].[ApiStats] WHERE [Instance] = @Instance AND [RequestYear] IN (@RequestYears) AND [RequestWeek] IN (@RequestWeeks) AND [RequestMethod] IN (@RequestMethods)" +
                    " AND [RequestPath] IN (@RequestPaths);";

                var parameters = new { Instance = instance, RequestYears = requestYears, RequestWeeks = requestWeeks, RequestMethods = requestMethods, RequestPaths = requestPaths };
                var stats = (await connection.QueryAsync<ApiStats>(sql, parameters)).ToList();

                return stats;
            }
        }

        public async Task AddAsync(List<ApiStats> stats)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO [Permillity].[ApiStats] ([Instance], [IsSuccess], [RequestYear], [RequestWeek], [RequestMethod], [RequestPath], [RequestCount], [AverageTime], [MaximumTime], [ExampleQueryString], [ExampleBody])" +
                    " VALUES (@Instance, @IsSuccess, @RequestYear, @RequestWeek, @RequestMethod, @RequestPath, @RequestCount, @AverageTime, @MaximumTime, @ExampleQueryString, @ExampleBody)";

                await connection.ExecuteAsync(sql, stats);
            }
        }

        public async Task UpdateAsync(List<ApiStats> stats)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE [Permillity].[ApiStats] SET [RequestWeek] = @RequestWeek, [RequestYear] = @RequestYear, [RequestMethod] = @RequestMethod, [RequestPath] = @RequestPath, " +
                    "[RequestCount] = @RequestCount, [AverageTime] = @AverageTime, [MaximumTime] = @MaximumTime, [ExampleQueryString] = @ExampleQueryString, [ExampleBody] = @ExampleBody WHERE [Id] = @Id";

                foreach (var stat in stats)
                {
                    await connection.ExecuteAsync(sql, stat);
                }
            }
        }

        private void CreatePermillityTables()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var createSchemaSql = "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Permillity') BEGIN EXEC('CREATE SCHEMA Permillity'); END";
                connection.Execute(createSchemaSql);

                var createTableSql = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Permillity' AND TABLE_NAME = 'ApiStats') BEGIN" +
                    " CREATE TABLE [Permillity].[ApiStats] ( Id INT IDENTITY(1,1) PRIMARY KEY,[Instance] UNIQUEIDENTIFIER NOT NULL, [IsSuccess] BIT NOT NULL, [RequestWeek] INT NOT NULL," +
                    " [RequestYear] INT NOT NULL, [RequestMethod] NVARCHAR(10) NOT NULL, [RequestPath] NVARCHAR(500) NOT NULL, [RequestCount] INT NOT NULL, [AverageTime] DECIMAL(8, 2) NOT NULL," +
                    " [MaximumTime] DECIMAL(8, 2) NOT NULL, [ExampleQueryString] NVARCHAR(1000) NULL, [ExampleBody] NVARCHAR(1000) NULL); END";
                connection.Execute(createTableSql);
            }
        }
    }
}
