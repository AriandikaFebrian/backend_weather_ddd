# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy file csproj dan restore dependencies
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Api/Api.csproj src/Api/
RUN dotnet restore src/Api/Api.csproj

# Copy semua file proyek dan build
COPY . .
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy hasil build dari stage 1
COPY --from=build-env /app/publish .

# Expose port dan jalankan aplikasi
EXPOSE 8000
CMD ["dotnet", "NetCa.Api.dll"]
