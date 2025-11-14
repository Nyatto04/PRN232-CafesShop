using CoffeeShop.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CoffeeShop.Web.Handlers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Lấy configuration

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- BẮT ĐẦU PHẦN CẤU HÌNH API ---

// 1. Đăng ký IHttpClientFactory
builder.Services.AddHttpClient();

// 1a. Đăng ký AuthTokenHandler với DI (Dependency Injection)
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

// Đăng ký tất cả các Service gọi API
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<ICartApiService, CartApiService>();
builder.Services.AddScoped<IOrderApiService, OrderApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();
builder.Services.AddScoped<IAdminApiService, AdminApiService>();
builder.Services.AddScoped<IReportApiService, ReportApiService>();
// (Chúng ta sẽ thêm IAdminService và IReportService vào đây sau)

// 5. Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Trang từ chối truy cập
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// --- KẾT THÚC PHẦN CẤU HÌNH API ---


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
// app.UseHttpsRedirection(); 
app.UseStaticFiles();

app.UseRouting();

// Cấu hình Middleware Pipeline
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// === PHẦN SỬA CHO BƯỚC 32 ===

// THÊM ROUTE CHO ADMIN AREA (PHẢI ĐẶT TRƯỚC ROUTE "default")
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route "default" (đã có)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// === KẾT THÚC SỬA ===

app.Run();