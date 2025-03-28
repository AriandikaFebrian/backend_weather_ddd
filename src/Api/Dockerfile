# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy semua source code dan build aplikasi
COPY . .  
RUN dotnet publish -c Release -o out  

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy hasil build dari stage 1
COPY --from=build-env /app/out .

# Expose port agar bisa diakses dari Koyeb
EXPOSE 8000

# Jalankan aplikasi dengan port yang sesuai dari environment Koyeb
CMD ["dotnet", "api.dll"]