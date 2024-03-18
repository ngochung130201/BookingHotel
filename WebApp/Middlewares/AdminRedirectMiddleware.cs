using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApp.Middlewares
{
    public class AdminRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/Admin/Login");
                return;
            }

            await _next(context);
        }
    }
}
