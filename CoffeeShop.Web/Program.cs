using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CoffeeShop.Web.Handlers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; 

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient("WebApi", client =>
{
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);

    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<ICartApiService, CartApiService>();
builder.Services.AddScoped<IOrderApiService, OrderApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();
builder.Services.AddScoped<IAdminApiService, AdminApiService>();
builder.Services.AddScoped<IReportApiService, ReportApiService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();