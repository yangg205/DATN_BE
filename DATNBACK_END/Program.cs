using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DATNBACK_END.Models;
using DATNBACK_END.Services;
using DATNBACK_END.Helpers;
using DATNBACK_END.AppDataContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load config từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Cấu hình dịch vụ
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// Cấu hình DbSetting
builder.Services.Configure<DbSetting>(builder.Configuration.GetSection("DbSetting"));
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));

// Kết nối database
builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbSetting:ConnectionString")));

// Đăng ký dịch vụ DI
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPlayerCharacterService, PlayerCharacterService>();
builder.Services.AddScoped<IPlayerCharacterSkillService, PlayerCharacterSkillService>();

// ✅ Thêm SignalR
builder.Services.AddSignalR();

// ✅ Thêm CORS (cho phép từ mọi nguồn)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Tạo DB scope nếu cần (hiện đoạn này chưa có tác dụng)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider;
}

// Middleware pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}

// Các middleware quan trọng cần đúng thứ tự
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();               // ✅ PHẢI đặt trước UseCors, UseSession, MapHub
app.UseCors();                  // ✅ CORS áp dụng toàn cục
app.UseSession();              // ✅ Session cần đặt sau UseRouting

// Nếu có xác thực thì thêm:
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();           // ✅ API
app.MapHub<GameHub>("/gamehub"); // ✅ SignalR Hub

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=LoginAndRegister}/{id?}");

app.Run();
