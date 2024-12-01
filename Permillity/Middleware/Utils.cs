using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Permillity.Middleware
{
    internal static class Utils
    {
        public static async Task<string> GetBodyAsync(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();

                using (var reader = new StreamReader(request.Body, default, true, 1024, leaveOpen:true))
                {
                    var body = await reader.ReadToEndAsync();

                    request.Body.Seek(0, SeekOrigin.Begin);

                    return string.IsNullOrEmpty(body) ? null : body;
                }
            }
            catch
            {
                return "Error parsing body";
            }
        }

        public static string GetPathTemplate(HttpContext context)
        {
            var routeTemplate = (context.Features.Get<IEndpointFeature>()?.Endpoint as RouteEndpoint)?.RoutePattern.RawText;

            return routeTemplate ?? context.Request.Path.Value;
        }

        public static string GetQueryString(HttpRequest request)
        {
            return request.Path.Value + request.QueryString.Value;
        }

        public static bool GetResponseSuccessStatus(HttpResponse response)
        {
            return response.StatusCode >= 200 && response.StatusCode < 300;
        }

        public static bool GetAvoidAttribute(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;

            var avoidAttribure = endpoint?.Metadata.OfType<PermillityAvoid>().FirstOrDefault();

            return avoidAttribure != null;
        }
    }
}
