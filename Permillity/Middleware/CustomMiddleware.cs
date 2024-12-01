using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Permillity.Trackers;

namespace Permillity.Middleware
{
    internal class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly Stopwatch _stopwatch;
        private readonly string[] _ignoreEndpoints;

        public CustomMiddleware(RequestDelegate next, PermillityOptions options)
        {
            _next = next;
            _ignoreEndpoints = options.IgnoreEndpoints;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, IPerformanceTracker _apiPerformanceService)
        {
            var avoidAttribute = Utils.GetAvoidAttribute(context);
            var path = Utils.GetPathTemplate(context);
            if (avoidAttribute || _ignoreEndpoints.Contains(path))
            {
                await _next(context);

                return;
            }

            var body = await Utils.GetBodyAsync(context.Request);
            _stopwatch.Restart();

            await _next(context);

            _stopwatch.Stop();
            var queryString = Utils.GetQueryString(context.Request);
            var isSuccess = Utils.GetResponseSuccessStatus(context.Response);

            _ = _apiPerformanceService.ProcessAsync(context.Request.Method, path, queryString, body, _stopwatch.ElapsedMilliseconds, isSuccess);
        }
    }
}
