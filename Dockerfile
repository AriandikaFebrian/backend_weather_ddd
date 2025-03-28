# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj dan restore dependencies
COPY src/Api/Api.csproj src/Api/
RUN dotnet restore src/Api/Api.csproj

# Copy semua source code dan build aplikasi
COPY . .  
RUN dotnet publish src/Api/Api.csproj -c Release -o src/Api/out  

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy hasil build dari stage 1
COPY --from=build-env /app/src/Api/out .

# Expose port agar bisa diakses
EXPOSE 8000

# Jalankan aplikasi
CMD ["dotnet", "NetCa.Api.dll"]
