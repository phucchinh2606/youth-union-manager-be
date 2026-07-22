using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.RepositoryImplement;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// THÊM ĐOẠN NÀY ĐỂ MỞ KHÓA CORS CHO NEXT.JS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Trỏ đúng cổng của Next.js
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyFrontend", policy =>
    {
        policy.WithOrigins("https://youth-union-manager-fe.vercel.app",
        "https://youth-union-manager-fe-phuc-chinhs-projects.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Rất quan trọng nếu bạn có dùng Token/Cookie đăng nhập
    });
});

// Đăng ký Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Đăng ký AppDbContext với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Cấu hình Authentication với JWT

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Gọi hàm tự động tạo Admin
        await DataSeeder.SeedAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Bỏ qua hoặc log lỗi nếu có trục trặc trong quá trình seed data
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
// Bật Swagger UI khi chạy ở môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChiDoanChauPhong API");
        c.RoutePrefix = string.Empty; // mở Swagger UI ngay tại root (http://localhost:2603)
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowMyFrontendNextJs");

app.UseAuthentication(); // Thêm dòng này (Xác thực xem token có hợp lệ không)
app.UseAuthorization();  // Thêm dòng này (Kiểm tra xem có quyền gọi API không)

app.MapControllers();
app.MapGet("/", () => "API Backend Quản lý Đoàn viên đang hoạt động tốt!");
app.Run();
