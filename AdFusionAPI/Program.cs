﻿using AdFusionAPI;
using AdFusionAPI.APIConfig;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Service;
using Supabase;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký các dịch vụ cơ bản
builder.Services.AddControllers(opt =>
{
    opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CloudinaryStorageService>();

// 2. Đăng ký DbContext với cấu hình kết nối tới PostgreSQL
builder.Services.AddDbContext<PostgresContext>(op =>
    op.UseNpgsql(builder.Configuration.GetConnectionString("AdFusionConnection")));

builder.Services.AddSingleton(provider =>
{
    var supabaseKey = builder.Configuration["Supabase:Key"];
    var supabaseUrl = builder.Configuration["Supabase:Url"];

    var options = new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true,
        SessionHandler = new DefaultSupabaseSessionHandler() //<-- This must be implemented by the developer
    };

    return new Client(supabaseUrl, supabaseKey, options);
});

// 4. Đăng ký các dịch vụ tùy chỉnh cho dự án
builder.Services.AddProjectServices();
builder.Services.AddQuartzServices();
//builder.Services.AddJwtAuthentication();

// 5. Cấu hình Swagger với JWT Bearer Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid token in the text input below.\r\n\r\nExample: \"TokenTest\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 6. Cấu hình JSON để bỏ qua các vòng tham chiếu và các thuộc tính chỉ đọc
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 7. Đăng ký AutoMapper với profile được định nghĩa sẵn
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 8. Cấu hình chính sách CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 9. Xây dựng ứng dụng (Application)
var app = builder.Build();

// 10. Cấu hình middleware cho môi trường phát triển
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// 11. Kích hoạt CORS, định tuyến, và middleware xác thực
app.UseCors("AllowAll");
app.UseRouting();
app.UseMiddleware<RequestLogMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<CheckBearerTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// 12. Map các controller để xử lý các yêu cầu API
app.MapControllers();

// 13. Chạy ứng dụng
app.Run();
