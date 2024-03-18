using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Authorization
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            // Customize the login path based on the current area
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.RedirectUri = "/Admin/Login";
            }
            else
            {
                context.RedirectUri = "/Identity/Login";
            }

            return base.RedirectToLogin(context);
        }
    }
}