using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// === 設定 HTTPS 與憑證 ===
// 假設你把憑證放在「專案根目錄」的 "certs/mycert.pfx"
var certPath = Path.Combine(AppContext.BaseDirectory, "certs", "cert.pfx");
var certPassword = "Samyahoo123"; // 請換成你自己的密碼

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps(certPath, certPassword);
    });

    // 如果需要同時支援 HTTP (例如用來導轉到 HTTPS)，可以加這個：
    // options.Listen(IPAddress.Any, 80);
});

builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // 強制 HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 設定預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
