# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Salin file csproj dari setiap proyek untuk melakukan restore dependensi
COPY src/Api/Api.csproj ./src/Api/
COPY src/Application/Application.csproj ./src/Application/
COPY src/Infrastructure/Infrastructure.csproj ./src/Infrastructure/
COPY src/Domain/Domain.csproj ./src/Domain/
RUN dotnet restore src/Api/Api.csproj || echo "Some packages failed to restore, continuing build."


# Salin seluruh kode sumber
COPY . .  

# Build dan publish aplikasi
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Salin hasil build dari stage 1
COPY --from=build-env /app/publish ./

# Expose port agar bisa diakses dari luar
EXPOSE 8000

# Jalankan aplikasi
CMD ["dotnet", "NetCa.Api.dll"]
