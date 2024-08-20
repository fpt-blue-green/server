using AdFusionAPI;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Service.Helper;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký các dịch vụ cơ bản và thiết lập cấu hình mặc định
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresContext>(op =>
    op.UseNpgsql(builder.Configuration.GetConnectionString("AdFusionConnection")));

// 2. Đăng ký các dịch vụ tùy chỉnh cho dự án
builder.Services.AddProjectServices();
builder.Services.AddQuartzServices();
builder.Services.AddJwtAuthentication();

// 3. Thiết lập chính sách ủy quyền (Authorization Policy)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

// 4. Cấu hình JSON để bỏ qua các vòng tham chiếu và các thuộc tính chỉ đọc
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    });
// 5. Đăng ký AutoMapper với profile được định nghĩa sẵn
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 6. Cấu hình chính sách CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 7. Xây dựng ứng dụng (Application)
var app = builder.Build();

// 8. Cấu hình middleware cho môi trường phát triển
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 9. Kích hoạt CORS, định tuyến, và middleware xác thực
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 10. Map các controller để xử lý các yêu cầu API
app.MapControllers();

// 11. Chạy ứng dụng
app.Run();
