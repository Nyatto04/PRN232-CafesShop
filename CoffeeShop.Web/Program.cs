using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Lấy configuration

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- THÊM PHẦN NÀY VÀO ---

// 1. Đăng ký IHttpClientFactory
builder.Services.AddHttpClient();

// 2. Tạo một "named client" (client có tên) để gọi WebApi
builder.Services.AddHttpClient("WebApi", client =>
{
    // Lấy BaseUrl từ file appsettings.json
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);

    // Thêm các Header mặc định nếu cần (ví dụ)
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 3. Đăng ký HttpContextAccessor để có thể đọc/ghi Cookie (lưu Token)
builder.Services.AddHttpContextAccessor();

// 4. Cấu hình Session (để lưu giỏ hàng tạm thời)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Trang từ chối truy cập
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// ... (phần code if env.IsDevelopment()...)

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();