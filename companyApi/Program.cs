using companyApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Minute)
    .CreateLogger();



builder.Services.AddSerilog();
builder.Host.UseSerilog();


// Add services to the container.

//builder.Services.AddControllers();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(); // ← This adds JsonPatchDocument support.



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();
// ← هذا يضيف دعم JsonPatchDocument


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();











//using companyApi.Models;
//using Microsoft.EntityFrameworkCore;
//using Serilog;

//try
//{
//    // 1. إعداد Serilog (قبل إنشاء الـ builder)
//    Log.Logger = new LoggerConfiguration()
//        .MinimumLevel.Information()
//        .WriteTo.Console()
//        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Minute)
//        .Enrich.FromLogContext()
//        .CreateLogger();

//    Log.Information("Starting web application...");

//    var builder = WebApplication.CreateBuilder(args);


//    builder.Host.UseSerilog();

//    // Add services to the container.
//    builder.Services
//        .AddControllers()
//        .AddNewtonsoftJson(); // لدعم JsonPatchDocument

//    builder.Services.AddEndpointsApiExplorer();
//    builder.Services.AddSwaggerGen();

//    builder.Services.AddDbContext<AppDbContext>(options =>
//        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//    var app = builder.Build();

//    // Configure the HTTP request pipeline.
//    if (app.Environment.IsDevelopment())
//    {
//        app.UseSwagger();
//        app.UseSwaggerUI();
//    }

//    // تسجيل الطلبات HTTP تلقائياً
//    app.UseSerilogRequestLogging();

//    app.UseHttpsRedirection();
//    app.UseAuthorization();
//    app.MapControllers();

//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Application terminated unexpectedly");
//}
//finally
//{
//    Log.CloseAndFlush();
//}
