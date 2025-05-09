namespace MyApp.Middleware
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;


    public class LoginRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _loginPath;

        public LoginRedirectMiddleware(RequestDelegate next, string loginPath)
        {
            _next = next;
            _loginPath = loginPath.ToLowerInvariant();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool isUserLoggedIn = !string.IsNullOrEmpty(context.Session.GetString("UserId"));
            string requestPath = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            if (!isUserLoggedIn && requestPath != _loginPath)
            {
                if (!requestPath.StartsWith("/lib/") &&
                    !requestPath.StartsWith("/css/") &&
                    !requestPath.StartsWith("/js/") &&
                    !requestPath.EndsWith(".css") &&
                    !requestPath.EndsWith(".js") &&
                    !requestPath.EndsWith(".ico"))
                {
                    context.Response.Redirect(_loginPath);
                    return;
                }
            }
            await _next(context);
        }
    

    }

    public static class LoginRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginRedirect(
            this IApplicationBuilder builder, string loginPath)
        {
            return builder.UseMiddleware<LoginRedirectMiddleware>(loginPath);
        }
    }
}