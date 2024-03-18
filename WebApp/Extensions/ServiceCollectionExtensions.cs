using BusinessLogic.Configurations;
using BusinessLogic.Contexts;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services.Common;
using BusinessLogic.Services.CronJob;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Authorization;
using WebApp.Helpers;
using WebApp.Services;

namespace WebApp.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static AppConfiguration GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            services.Configure<MailConfiguration>(configuration.GetSection(nameof(MailConfiguration)));
            //services.AddSingleton<MailConfiguration>();
            //services.Configure<MailConfiguration>(config =>
            //{
            //    using var scope = services.BuildServiceProvider().CreateScope();
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    config.SetValuesFromDatabase(dbContext);
            //});
            return applicationSettingsConfiguration.Get<AppConfiguration>() ?? throw new InvalidOperationException();
        }

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<ApplicationDbContext>(options => options
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? string.Empty))
                .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        internal static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentity<AppUser, AppRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;

                    // User settings
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Reset token after 1 hour
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(1));

            //add session
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromHours(2);
                option.Cookie.HttpOnly = true;
            });

            // Add application services.
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            // Add user claim
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();

            //Add config
            services.AddTransient<IAuthorizationHandler, BaseResourceAuthorizationHandler>();
            services.AddSingleton<CustomCookieAuthenticationEvents>();
            services.ConfigureApplicationCookie(options =>
            {
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                googleOptions.CallbackPath = "/identity/login-google";
            });

            return services;
        }

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        /// <summary>
        /// Add HangFire Extension
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configuration"></param>
        public static void AddHangFireExtension(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHangfire(x =>
            {
                x.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"));
            });
            service.AddHangfireServer();
        }

        /// <summary>
        /// Schedule Job
        /// </summary>
        /// <param name="service"></param>
        public static void AddScheduleJob(this IServiceCollection service)
        {
            service.AddScoped<IBackgroundJobService, BackgroundJobService>();
        }
    }
}