FROM mcr.microsoft.com/dotnet/8.0-aspnet:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/8.0-sdk:latest AS build
WORKDIR /src
COPY ["src/Api/Api.csproj", "src/Api/"]
RUN dotnet restore "src/Api/Api.csproj"

COPY . .
RUN dotnet publish "src/Api/Api.csproj" -c Release -o /app/out

FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "NetCa.Api.dll"]
