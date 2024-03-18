using System.Globalization;
using BusinessLogic.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using WebApp.Extensions;
using WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
        opts => { opts.ResourcesPath = "Resources"; })
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new LowercaseFirstCharNamingPolicy();
    });
//Add localization
builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
//Add culture for localization
builder.Services.Configure<RequestLocalizationOptions>(
    opts =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new("en-US"),
            new("vi-VN")
        };

        foreach (var item in supportedCultures)
        {
            item.NumberFormat.CurrencySymbol = "$";
        }

        opts.DefaultRequestCulture = new RequestCulture("en-US");
        // Formatting numbers, dates, etc.
        opts.SupportedCultures = supportedCultures;
        // UI strings that we have localized.
        opts.SupportedUICultures = supportedCultures;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddSignalR();
builder.Services.AddCurrentUserService();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddHangFireExtension(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(24);
    }
);
// Add infrastructure layer
builder.Services.AddInfrastructureMappings();
// Add services
builder.Services.AddServices();
builder.Services.GetApplicationSettings(builder.Configuration);
builder.Services.AddScheduleJob();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Admin/Error404");
}
app.UseSession();
app.UseStaticFiles();
// Chuyển hướng đến /admin/login khi truy cập trang chủ
app.UseMiddleware<WebApp.Middlewares.AdminRedirectMiddleware>();

app.UseRouting();
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    DashboardTitle = "Background Jobs",
    //Authorization = new[] { new HangfireAuthorizationFilter() }
});
app.UseAuthentication();
app.UseAuthorization();
app.UseLocalization();
app.UseEndpoints();
app.Initialize();
app.Run();
