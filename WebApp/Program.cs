using WebApp.Domain;
using WebApp.Services;
using WebApp.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpCredetials>(
    builder.Configuration.GetSection(nameof(SmtpCredetials)));

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Catalog<Product>>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductCatalog}/{action=Products}");

app.Run();