using AdFusionAPI;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Service.Implement;
using Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresContext>(op => op.UseNpgsql(builder.Configuration.GetConnectionString("AdFusionConnection")));

builder.Services.AddProjectServices();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    });

// Add services to the container.
var serviceProvider = builder.Services.BuildServiceProvider();
var systemSetting = serviceProvider.GetRequiredService<ISystemSettingService>();
var jwtSettings = systemSetting.GetJWTSystemSetting();
var key = jwtSettings.Result.KeyValue;
builder.Services.AddJwtAuthentication(key!);


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
