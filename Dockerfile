# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Salin hanya file csproj dari API dan lakukan restore dependensi
COPY src/Api/*.csproj ./src/Api/
RUN dotnet restore src/Api/Api.csproj

# Salin seluruh source code (termasuk projek lainnya) dan build aplikasi
COPY . .  
RUN dotnet publish src/Api/Api.csproj -c Release -o out  

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Salin hasil build dari stage 1
COPY --from=build-env /app/out ./

# Expose port agar bisa diakses
EXPOSE 8000

# Jalankan aplikasi API
CMD ["dotnet", "NetCa.Api.dll"]
