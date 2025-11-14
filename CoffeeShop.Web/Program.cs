using CoffeeShop.Web.Services;

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

// --- KẾT THÚC PHẦN THÊM VÀO ---


var app = builder.Build();

// ... (phần code if env.IsDevelopment()...)

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// THÊM DÒNG NÀY (để dùng Session)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();