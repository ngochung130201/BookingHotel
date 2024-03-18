using BusinessLogic.Services.Common;
using BusinessLogic.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BusinessLogic.Services;

namespace BusinessLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

            // Identity
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IAccountService, Services.Identity.AccountService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IFunctionService, FunctionService>();
            // User
            services.AddTransient<IUserService, UserService>();
            // Pages
            services.AddTransient<IPagesService, PagesService>();
            // Partner
            services.AddTransient<IPartnerService, PartnerService>();
            // Invoice
            services.AddTransient<IInvoiceService, InvoiceService>();
            // DashboardService
            services.AddTransient<IDashboardService, DashboardService>();

            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });
            });
        }
    }
}