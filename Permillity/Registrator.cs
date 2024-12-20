﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Permillity.Dashboard;
using Permillity.Database;
using Permillity.Middleware;
using Permillity.Trackers;
using System;

namespace Permillity
{
    public static class Registrator
    {
        /// <summary>
        /// Register permillity services.
        /// </summary>
        /// <param name="services">Application IServiceCollection</param>
        /// <param name="buildAction">Options actions. Allows you to set different options for permillity.</param>
        /// <returns></returns>
        public static IServiceCollection AddPermillity(this IServiceCollection services, Action<PermillityOptionsBuilder> buildAction = null)
        {
            var options = new PermillityOptions();

            if (buildAction != null)
            {
                var builder = new PermillityOptionsBuilder();
                buildAction(builder);
                options = builder.Build();
            }

            services.AddSingleton(options);
            services.AddSingleton<Coordinator>();
            services.AddSingleton<LocalStorage>();
            services.AddScoped<IPerformanceTracker, PerformanceTracker>();
            services.AddScoped<IPermillityService, PermillityService>();

            if (options.UseInMemory)
                services.AddSingleton<IRepository, InMemoryRepository>();
            else if (options.UseMySql)
                services.AddScoped<IRepository, MySqlRepository>();
            else if (options.UseSqlite)
                services.AddScoped<IRepository, SqliteRepository>();
            else
                services.AddScoped<IRepository, SqlServerRepository>();

            return services;
        }

        /// <summary>
        /// Adds permillity middleware for logging request performance.
        /// </summary>
        /// <param name="app">Instance of WebApplication.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddPermillityMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomMiddleware>();
        }
    }
}
