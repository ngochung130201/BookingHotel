using BusinessLogic.Services.Common;
using Microsoft.Extensions.Options;

namespace WebApp.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        internal static void UseEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Login}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                   name: "AdminError",
                   pattern: "Admin/{*catchall}",
                   defaults: new { area = "Admin", controller = "Error404", action = "Index" }
               );
                // Cấu hình tuyến đường cho các yêu cầu không khớp khác
                endpoints.MapFallbackToController("Index", "Home");
            });
        }

        internal static void UseLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions!.Value);
        }

        internal static void Initialize(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }
        }
    }
}