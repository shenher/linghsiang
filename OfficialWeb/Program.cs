using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OfficialWeb.Models.Settings;
using OfficialWeb.Services;
using OfficialWeb.Tools;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var certPath = Path.Combine(AppContext.BaseDirectory, "certs", "cert.pfx");
    var configPassword = builder.Configuration["CertificatePassword"];

    var certPassword = !string.IsNullOrWhiteSpace(configPassword) ? configPassword
        : Environment.GetEnvironmentVariable("CERT_PASSWORD")
        ?? throw new InvalidOperationException(
            "Certificate password is not configured. Set 'CertificatePassword' in appsettings or CERT_PASSWORD environment variable.");

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Any, 443, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    });
}

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// 註冊電子郵件服務（Transient：每次注入都建立新實例，適合短暫的 SMTP 連線）
builder.Services.AddTransient<IEmailService, EmailService>();

// 全站站台設定（字體等，Options Pattern）
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"));

// 後台設定（登入密碼；環境變數 ADMIN_PASSWORD 可覆蓋，見 AdminController）
builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));

// 資料檔路徑：未設定 DataRoot 時用專案根目錄；Docker 以環境變數 DataRoot 指到掛載的 /app/data
builder.Services.AddSingleton<IDataPaths, DataPathService>();

// 菜單主檔服務（Singleton：內部以 lock 序列化 Menu.xlsx 讀寫）
builder.Services.AddSingleton<IMenuService, MenuExcelService>();

// 後台 Cookie 驗證（簡單密碼登入）
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// 啟動防呆：Menu.xlsx 或 Pic/ 缺件時自動建立
app.Services.GetRequiredService<IMenuService>().EnsureSeeded();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// === 安全性 HTTP 標頭 ===
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 設定預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
