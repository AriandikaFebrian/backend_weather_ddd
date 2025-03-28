# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy semua file csproj agar restore berjalan dengan benar
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Api/Api.csproj src/Api/

# Restore dependencies
RUN dotnet restore src/Api/Api.csproj

# Copy seluruh source code ke dalam container
COPY . . 

# Build aplikasi
RUN dotnet publish src/Api/Api.csproj -c Release -o src/Api/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy hasil build dari stage 1
COPY --from=build-env /app/src/Api/out . 

# Expose port
EXPOSE 8000

# Jalankan aplikasi
CMD ["dotnet", "NetCa.Api.dll"]
