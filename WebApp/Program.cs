using WebApp.Domain;
using WebApp.Services;
using WebApp.DAL;
using Serilog;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestHeaders &
                            HttpLoggingFields.RequestBody &
                            HttpLoggingFields.ResponseHeaders &
                            HttpLoggingFields.ResponseBody;
});

builder.Services.Configure<SmtpConfig>(
    builder.Configuration.GetSection(nameof(SmtpConfig)));

builder.Services.Configure<ServerStatusNotificationConfig>(
    builder.Configuration.GetSection(nameof(ServerStatusNotificationConfig)));

builder.Services.Configure<ProductAddedEventHandlerConfig>(
    builder.Configuration.GetSection(nameof(ProductAddedEventHandlerConfig)));

builder.Services.Configure<UserAgentFilterConfig>(
    builder.Configuration.GetSection(nameof(UserAgentFilterConfig)));

builder.Services.AddHostedService<ServerStatusNotificationService>();
builder.Services.AddHostedService<ProductAddedEventHandler>();
builder.Services.AddSingleton<Catalog<Product>>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IEmailSender, MailKitSmtpEmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseHttpLogging();

app.UseMiddleware<UserAgentFilterMiddleware>();

app.UseMiddleware<PageHitCounterMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();  