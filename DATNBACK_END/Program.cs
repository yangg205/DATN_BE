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

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var environment = builder.Environment;

// Thêm cấu hình từ `appsettings.json` và `appsettings.Development.json`
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

builder.Services.Configure<DbSetting>
    (builder.Configuration.GetSection("DbSetting"));
builder.Services.Configure<EmailSetting>
    (builder.Configuration.GetSection("EmailSetting"));
// builder.Services.AddSingleton<AppDataContext>();

builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbSetting:ConnectionString")));


builder.Services.AddScoped<IPlayerService,PlayerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPlayerCharacterService,PlayerCharacterService>();

// "DbSetting": {
//     "ConnectionString": "Data Source=SQL1001.site4now.net;Initial Catalog=db_ab18c1_3dnangcao;User Id=db_ab18c1_3dnangcao_admin;Password=yangg2206"
//   },

var app = builder.Build();

{
    using var score = app.Services.CreateScope();
    var context = score.ServiceProvider;
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Swagger hiển thị tại /swagger
    });
}


app.UseHttpsRedirection();
app.MapControllers();

app.UseCors(x=>x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=LoginAndRegister}/{id?}");

app.UseSession();
app.Run();
