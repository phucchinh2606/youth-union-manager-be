# 1. Dùng ảnh SDK của .NET 9 để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2. Copy file Solution và các file .csproj để restore thư viện
COPY ["YouthUnionManager.slnx", "./"]
COPY ["API/API.csproj", "API/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
# Nếu có thư mục Core hay Application thì bỏ comment và sửa 2 dòng dưới:
# COPY ["Core/Core.csproj", "Core/"]
# COPY ["Application/Application.csproj", "Application/"]

RUN dotnet restore "YouthUnionManager.slnx"

# 3. Copy toàn bộ mã nguồn còn lại và tiến hành Build
COPY . .
WORKDIR "/src/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# 4. Chuyển sang ảnh Runtime nhẹ hơn để chạy ứng dụng (tiết kiệm RAM trên Render)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# 5. Cấu hình cổng mạng (Render thường dùng cổng 8080 cho Web Service)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# 6. Lệnh khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "API.dll"]