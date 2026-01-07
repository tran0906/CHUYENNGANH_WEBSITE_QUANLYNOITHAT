// FILE: Program.cs - Điểm khởi động ứng dụng ASP.NET Core MVC

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MVC
builder.Services.AddControllersWithViews();

// Cấu hình Session - lưu thông tin tạm thời (giỏ hàng, đăng nhập)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".NoiThatViet.Session";
});

// Cấu hình Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Cấu hình Authentication - xác thực người dùng bằng Cookie
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = ".NoiThatViet.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Cookie hết hạn sau 7 ngày
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Cấu hình middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép truy cập file tĩnh (wwwroot)
app.UseRouting();
app.UseCookiePolicy();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Cấu hình routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"); // Route cho Admin Area

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Route mặc định

app.Run();
