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
            // DashboardService
            services.AddTransient<IDashboardService, DashboardService>();
            // RoomTypesService
            services.AddTransient<IRoomTypesService, RoomTypesService>();
            // ServicesServices
            services.AddTransient<IServicesServices, ServicesServices>();
            // RoomsServices
            services.AddTransient<IRoomsService, RoomsService>();
            // NewsService
            services.AddTransient<INewsService, NewsService>();
            // FeedBackService
            services.AddTransient<IFeedBackService, FeedBackService>();
            // VoteBookingServices
            services.AddTransient<IVoteBookingServices, VoteBookingServices>();
            // CostBookingService
            services.AddTransient<ICostBookingService, CostBookingService>();
            // BookingService
            services.AddTransient<IBookingService, BookingService>();
            // SpecialDayRatesService
            services.AddTransient<ISpecialDayRatesService, SpecialDayRatesService>();
            // ReplyCommentService
            services.AddTransient<IReplyCommentService, ReplyCommentService>();
            // CommentService
            services.AddTransient<ICommentService, CommentService>();
            // ExcelService
            services.AddTransient<IExcelService, ExcelService>();
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