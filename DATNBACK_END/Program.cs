using DATNBACK_END.AppDataContexts;
using DATNBACK_END.Helpers;
using DATNBACK_END.Models;
using DATNBACK_END.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));
builder.Services.Configure<DbSetting>
    (builder.Configuration.GetSection("DbSetting"));

// builder.Services.AddSingleton<AppDataContext>();
builder.Services.AddDbContext<AppDataContext>(option=>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DbSetting:ConnectionString"))); //tao 1 instance duy nhat cua appdatacontext

builder.Services.AddScoped<IPlayerService,PlayerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.AddHostedService<OtpCleanupService>();



var app = builder.Build();  

{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors(
    x=>x.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
);
app.UseStaticFiles();
app.UseRouting();
app.Run();
//API key của gg để tích hợp đăng nhập bằng gg
//AIzaSyBwLd_wPwYqChqE4mDXJLQZYXT124unTm4 
//Client secret: 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
//Client ID: 1058105992725-vqbmi2i0cn2na1d3kv6fhb4av0a30dpk.apps.googleusercontent.com
