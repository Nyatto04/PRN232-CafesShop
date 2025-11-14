using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CoffeeShop.Web.Handlers; // <--- Cần using này

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Lấy configuration

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- BẮT ĐẦU PHẦN CẤU HÌNH API ---

// 1. Đăng ký IHttpClientFactory
builder.Services.AddHttpClient();

// 1a. Đăng ký AuthTokenHandler với DI (Dependency Injection)
// DÒNG MỚI (1/3)
builder.Services.AddTransient<AuthTokenHandler>();

// 2. Tạo một "named client" (client có tên) để gọi WebApi
builder.Services.AddHttpClient("WebApi", client =>
{
    // Lấy BaseUrl từ file appsettings.json
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);

    // Thêm các Header mặc định nếu cần (ví dụ)
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
    // BẢO HTTPCLIENT SỬ DỤNG HANDLER ĐỂ TỰ ĐỘNG GẮN TOKEN
    // DÒNG MỚI (2/3)
    .AddHttpMessageHandler<AuthTokenHandler>();

// 3. Đăng ký HttpContextAccessor để có thể đọc/ghi Cookie (lưu Token)
builder.Services.AddHttpContextAccessor();

// 4. Cấu hình Session (để lưu giỏ hàng tạm thời)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đăng ký các Service gọi API
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();

// ĐĂNG KÝ SERVICE GIỎ HÀNG MỚI
// DÒNG MỚI (3/3)
builder.Services.AddScoped<ICartApiService, CartApiService>();

// 5. Cấu hình Cookie Authentication 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Trang từ chối truy cập
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
// app.UseHttpsRedirection(); // Bạn đã comment, rất tốt
app.UseStaticFiles();

app.UseRouting();

// Cấu hình Middleware Pipeline
// (Thứ tự này rất quan trọng)
app.UseAuthentication(); // 1. Xác định bạn là ai
app.UseSession();        // 2. Tải Session
app.UseAuthorization();  // 3. Kiểm tra bạn được làm gì

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();