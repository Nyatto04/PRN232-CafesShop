# 📚 TÀI LIỆU GIẢI THÍCH CẤU TRÚC BACKEND - CAFE SHOP PROJECT

## 🏗️ 1. KIẾN TRÚC TỔNG QUAN (LAYERED ARCHITECTURE)

Project sử dụng **kiến trúc phân lớp (Layered Architecture)** với 4 layers chính:

```
┌─────────────────────────────────────────┐
│         WebApi (Presentation Layer)     │  ← Controllers, API Endpoints
├─────────────────────────────────────────┤
│         Services (Business Layer)      │  ← Business Logic, Validation
├─────────────────────────────────────────┤
│         DAL (Data Access Layer)         │  ← Database Context, Models
├─────────────────────────────────────────┤
│         Shared (Common Layer)            │  ← DTOs, Settings
└─────────────────────────────────────────┘
```

### **1.1. WebApi Layer (Presentation Layer)**
- **Vai trò**: Nhận HTTP requests, xử lý routing, authentication/authorization
- **Thành phần**:
  - `Controllers/`: Xử lý HTTP requests/responses
  - `Program.cs`: Cấu hình ứng dụng, Dependency Injection, Middleware
  - `appsettings.json`: Cấu hình (Connection String, JWT, Mail)

### **1.2. Services Layer (Business Layer)**
- **Vai trò**: Chứa business logic, validation, xử lý nghiệp vụ
- **Thành phần**:
  - `Interfaces/`: Định nghĩa contracts (IProductService, ICartService...)
  - `Implementations/`: Implement các interface (ProductService, CartService...)

### **1.3. DAL Layer (Data Access Layer)**
- **Vai trò**: Tương tác với Database, định nghĩa Models
- **Thành phần**:
  - `Models/`: Entity classes (Product, Category, CartItem, ApplicationUser...)
  - `Data/ApplicationDbContext.cs`: EF Core DbContext
  - `Migrations/`: Database migrations (Code-First)

### **1.4. Shared Layer (Common Layer)**
- **Vai trò**: Chứa các class dùng chung giữa các layers
- **Thành phần**:
  - `Dtos/`: Data Transfer Objects (ProductDto, CategoryDto...)
  - `Settings/`: Configuration classes (MailSettings)

---

## 🔄 2. CÁC LUỒNG CHẠY CHÍNH

### **LUỒNG 1: ĐĂNG KÝ TÀI KHOẢN (Register Flow)**

```
Client Request
    ↓
[AuthController.Register]
    ↓
[AuthService.RegisterAsync]
    ↓
1. Kiểm tra email đã tồn tại?
    ↓ (Nếu chưa tồn tại)
2. Tạo ApplicationUser mới
    ↓
3. Hash password (Identity tự động)
    ↓
4. Lưu vào Database (UserManager.CreateAsync)
    ↓
5. Tự động tạo Roles nếu chưa có (Customer, Staff, Admin)
    ↓
6. Gán Role "Customer" cho user mới
    ↓
7. Tạo Email Confirmation Token
    ↓
8. Gửi email xác thực (MailService)
    ↓
9. Trả về BaseResponseDto { Success, Message }
```

**Code Flow:**
- `WebApi/Controllers/AuthController.cs` → `Register()`
- `Services/Implementations/AuthService.cs` → `RegisterAsync()`
- `DAL/Models/ApplicationUser.cs` → Entity model
- `Services/Implementations/MailService.cs` → Gửi email

---

### **LUỒNG 2: ĐĂNG NHẬP (Login Flow)**

```
Client Request (Email + Password)
    ↓
[AuthController.Login]
    ↓
[AuthService.LoginAsync]
    ↓
1. Tìm user theo Email (UserManager.FindByEmailAsync)
    ↓
2. Kiểm tra Password (UserManager.CheckPasswordAsync)
    ↓ (Nếu đúng)
3. Kiểm tra EmailConfirmed = true?
    ↓ (Nếu đã confirm)
4. Kiểm tra IsActive = true?
    ↓ (Nếu active)
5. Lấy Roles của user (UserManager.GetRolesAsync)
    ↓
6. Tạo JWT Token (CreateJwtToken)
    ├─ Claims: Name, Email, UserId, Roles
    ├─ Issuer: từ appsettings.json
    ├─ Audience: từ appsettings.json
    ├─ Expires: 8 giờ
    └─ Signing: HMAC SHA256
    ↓
7. Trả về LoginResponseDto { UserId, FullName, Email, Roles, Token, Expires }
```

**Code Flow:**
- `WebApi/Controllers/AuthController.cs` → `Login()`
- `Services/Implementations/AuthService.cs` → `LoginAsync()` → `CreateJwtToken()`
- JWT được tạo với claims: `uid`, `Role`, `Email`, `Name`

---

### **LUỒNG 3: XÁC THỰC JWT TOKEN (Authentication Flow)**

```
Client Request với Header: Authorization: Bearer <token>
    ↓
[Middleware Pipeline]
    ↓
1. UseCors() - Cho phép CORS
    ↓
2. UseAuthentication() - Xác thực JWT
    ├─ Đọc token từ Header
    ├─ Validate Issuer, Audience
    ├─ Validate Signature (JWT:Key)
    ├─ Extract Claims (UserId, Roles)
    └─ Set User.Identity với Claims
    ↓
3. UseAuthorization() - Kiểm tra quyền
    ├─ [Authorize] → Chỉ cần đăng nhập
    └─ [Authorize(Roles = "Customer")] → Cần Role cụ thể
    ↓
4. Controller nhận request với User đã authenticated
```

**Code Flow:**
- `WebApi/Program.cs` → `AddJwtBearer()` → Cấu hình validation
- `WebApi/Program.cs` → `UseAuthentication()` → Middleware xác thực
- Controller → `[Authorize]` attribute → Kiểm tra quyền

---

### **LUỒNG 4: LẤY DANH SÁCH SẢN PHẨM (Get Products - Guest)**

```
Client Request: GET /api/products
    ↓
[ProductsController.GetAllProducts]
    ↓ (Không cần Auth - Guest accessible)
[ProductService.GetAllProductsAsync]
    ↓
1. Query Database (EF Core)
    ├─ Filter: IsActive = true
    ├─ Include: Category (Join)
    └─ Select: Map to ProductDto
    ↓
2. Trả về BaseResponseDto
    {
        Result: Success,
        Message: "Thành công",
        Data: [ProductDto, ProductDto, ...]
    }
```

**Code Flow:**
- `WebApi/Controllers/ProductsController.cs` → `GetAllProducts()`
- `Services/Implementations/ProductService.cs` → `GetAllProductsAsync()`
- `DAL/Data/ApplicationDbContext.cs` → `Products` DbSet
- `DAL/Models/Product.cs` → Entity
- `Shared/Dtos/ProductDtos/ProductDto.cs` → DTO

---

### **LUỒNG 5: THÊM SẢN PHẨM VÀO GIỎ HÀNG (Add to Cart - Customer)**

```
Client Request: POST /api/cart
Header: Authorization: Bearer <token>
Body: { ProductId, Quantity, Size }
    ↓
[CartController.AddToCart]
    ↓
1. [Authorize(Roles = "Customer")] → Kiểm tra Role
    ↓ (Nếu có Role Customer)
2. Extract UserId từ JWT Token (GetCurrentUserId)
    ├─ User.FindFirstValue("uid")
    └─ Lấy từ Claims trong token
    ↓
3. Validate ModelState
    ↓
[CartService.AddToCartAsync]
    ↓
4. Kiểm tra Product tồn tại và IsActive?
    ↓ (Nếu có)
5. Kiểm tra Stock >= Quantity?
    ↓ (Nếu đủ)
6. Kiểm tra CartItem đã tồn tại? (UserId + ProductId + Size)
    ├─ Nếu có → Update Quantity
    └─ Nếu chưa → Create mới
    ↓
7. SaveChangesAsync()
    ↓
8. Trả về BaseResponseDto { Success, Message }
```

**Code Flow:**
- `WebApi/Controllers/CartController.cs` → `AddToCart()` → `GetCurrentUserId()`
- `Services/Implementations/CartService.cs` → `AddToCartAsync()`
- `DAL/Models/CartItem.cs` → Entity
- JWT Token → Claims → UserId

---

### **LUỒNG 6: TẠO SẢN PHẨM MỚI (Admin - Create Product)**

```
Client Request: POST /api/admin/products
Header: Authorization: Bearer <admin_token>
Body: { ProductName, Price, CategoryId, ... }
    ↓
[Admin/ProductsController.CreateProduct]
    ↓
1. [Authorize(Roles = "Admin")] → Kiểm tra Role Admin
    ↓ (Nếu có Role Admin)
2. Validate ModelState
    ↓
[ProductService.CreateProductAsync]
    ↓
3. Kiểm tra CategoryId tồn tại?
    ↓ (Nếu có)
4. Map CreateUpdateProductDto → Product Entity
    ↓
5. _context.Products.Add(product)
    ↓
6. SaveChangesAsync()
    ↓
7. Trả về BaseResponseDto { Success, Message, Data: Product }
```

**Code Flow:**
- `WebApi/Controllers/Admin/ProductsController.cs` → `CreateProduct()`
- `Services/Implementations/ProductService.cs` → `CreateProductAsync()`
- `Shared/Dtos/ProductDtos/CreateUpdateProductDto.cs` → Input DTO
- `DAL/Models/Product.cs` → Entity

---

## 🔐 3. AUTHENTICATION & AUTHORIZATION

### **3.1. JWT Token Structure**
```json
{
  "iss": "http://localhost:5146",        // Issuer
  "aud": "http://localhost:5146",        // Audience
  "exp": 1234567890,                     // Expiration
  "claims": {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "email@example.com",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email": "email@example.com",
    "uid": "user-id-string",              // Custom claim
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Customer"
  }
}
```

### **3.2. Authorization Levels**
- **Guest (No Auth)**: Categories, Products APIs
- **Customer (Role = "Customer")**: Cart APIs
- **Admin (Role = "Admin")**: Admin APIs (Products, Categories, Orders, Reports)

---

## 📦 4. DEPENDENCY INJECTION (DI)

### **4.1. Service Registration (Program.cs)**
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
// ...
```

### **4.2. Constructor Injection**
- Controllers nhận Services qua constructor
- Services nhận DbContext qua constructor
- Tự động resolve bởi .NET DI Container

---

## 🗄️ 5. DATABASE (Code-First Approach)

### **5.1. Models (Entities)**
- `ApplicationUser`: User (kế thừa IdentityUser)
- `ApplicationRole`: Role (kế thừa IdentityRole)
- `Product`: Sản phẩm
- `Category`: Danh mục
- `CartItem`: Giỏ hàng
- `Order`, `OrderItem`: Đơn hàng

### **5.2. Relationships**
- `Product` → `Category` (Many-to-One)
- `CartItem` → `User` (Many-to-One)
- `CartItem` → `Product` (Many-to-One)

### **5.3. Migrations**
- Tạo migration: `dotnet ef migrations add MigrationName`
- Apply: `dotnet ef database update`
- Location: `DAL/Migrations/`

---

## 📊 6. RESPONSE FORMAT (Standardized)

Tất cả API trả về cùng format:
```json
{
  "result": 1,              // ResultValue enum: 1=Success, 0=Failed, -1=NoData
  "message": "Thành công",  // Thông báo
  "data": {}                // Dữ liệu (object, array, hoặc null)
}
```

---

## 🔧 7. MIDDLEWARE PIPELINE (Request Flow)

```
1. CORS (UseCors)
    ↓
2. HTTPS Redirection (nếu có)
    ↓
3. Authentication (UseAuthentication)
    ├─ Đọc JWT Token
    ├─ Validate Token
    └─ Set User.Identity
    ↓
4. Authorization (UseAuthorization)
    ├─ Kiểm tra [Authorize]
    └─ Kiểm tra Roles
    ↓
5. Controllers
    ├─ Validate ModelState
    ├─ Gọi Service
    └─ Trả về Response
```

---

## 📝 8. CÁC ĐIỂM QUAN TRỌNG KHI REVIEW

1. **Separation of Concerns**: Mỗi layer có trách nhiệm riêng
2. **Dependency Injection**: Loose coupling giữa các components
3. **DTO Pattern**: Tách biệt Entity và DTO để bảo mật
4. **Repository Pattern**: Services layer che giấu database access
5. **JWT Authentication**: Stateless authentication
6. **Role-Based Authorization**: Phân quyền theo Role
7. **Code-First**: Database được tạo từ Models
8. **Standardized Response**: Tất cả API dùng BaseResponseDto

---

## 🎯 9. TÓM TẮT LUỒNG ĐI CỦA MỘT REQUEST

```
HTTP Request
    ↓
Middleware (CORS, Auth, Authorization)
    ↓
Controller (Validate, Extract UserId từ Token nếu cần)
    ↓
Service (Business Logic, Validation)
    ↓
DbContext (Database Query/Command)
    ↓
Service (Map Entity → DTO)
    ↓
Controller (Return BaseResponseDto)
    ↓
HTTP Response (JSON)
```

---

**Tài liệu này giúp bạn hiểu rõ cấu trúc và luồng chạy của Backend để giải thích với thầy khi review!** 🎓

