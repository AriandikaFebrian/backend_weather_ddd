#Part 1
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS base

ARG MAJOR_MINOR=0.1
ARG PATCH=0
ARG SUFFIX_VERSION=-DEV

# Build app from sdk image
WORKDIR /src
COPY . .
RUN dotnet publish "NetCa.sln" -c Release -o ./app /p:VersionPrefix=$MAJOR_MINOR.$PATCH /p:VersionSuffix=$SUFFIX_VERSION

#Part 2
FROM utseadadrndnpacr.azurecr.io/dotnet/8-noble:latest AS final

# Copy to runtime image
ENV HOME=/home/ut-dad
WORKDIR $HOME/app
COPY --from=base /src/app .

ENTRYPOINT ["dotnet", "NetCa.Api.dll"]
