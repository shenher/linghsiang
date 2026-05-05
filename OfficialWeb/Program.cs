using Microsoft.AspNetCore.Mvc;
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

var app = builder.Build();

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

app.UseAuthorization();

// 設定預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
