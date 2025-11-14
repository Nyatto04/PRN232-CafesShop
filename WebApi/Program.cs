using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Implementations;
using Services.Interfaces;
using Shared.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- 1. Đăng ký DbContext (Kết nối CSDL) ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("MyCnn"),
        // Bảo đảm DbContext nằm trong project DAL
        b => b.MigrationsAssembly("DAL")));

// --- 2. Đăng ký Identity (User, Role) ---
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Yêu cầu xác thực email
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Cần cho việc tạo token (reset pass, confirm email)

// --- 3. Đăng ký Cấu hình Mail ---
builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

// --- 4. Đăng ký Service (DI - Dependency Injection) ---
// Đăng ký các Service từ project Services
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddHttpContextAccessor(); // Cần để lấy URL (tạo link confirm) - CHỈ CẦN 1 DÒNG

// --- 5. Cấu hình Xác thực (Authentication) - Rất quan trọng ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Cấu hình JWT Bearer (cho API)
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Tắt HTTPS (khi dev), BẬT (true) khi production
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:Audience"],
        ValidIssuer = configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
    };
});

// --- 6. Cấu hình CORS (Cho phép Frontend gọi API) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Cho phép mọi domain (thay bằng domain frontend khi production)
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// --- 7. Các Service có sẵn của WebApi ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 8. Cấu hình Swagger (để Test API) ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoffeeShop API", Version = "v1" });

    // Cấu hình để Swagger có thể gửi JWT (nút Authorize)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Nhập 'Bearer ' [khoảng trắng] rồi dán token vào. \r\n\r\n 
                          Ví dụ: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


// === BUILD APP ===
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Gọi hàm Seed từ DbSeeder
        await DbSeeder.SeedRolesAndAdminAsync(services);
        logger.LogInformation("Khởi tạo dữ liệu (seed) Role và Admin thành công.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo dữ liệu (seeding).");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop API v1"));
}

// Bật CORS
app.UseCors("AllowAll");

// Bật HTTPS (nếu có)
// app.UseHttpsRedirection();

// Bật Xác thực và Phân quyền
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();