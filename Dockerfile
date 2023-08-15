# syntax=docker/dockerfile:1
# Build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src

# Copy fsproj and restore as distinct layers
COPY . ./
RUN dotnet publish Fubernetes.Main/ -c Release -o out

# ========================================================
# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build-env /src/out .
ENV RUN_ENV="PRODUCTION"

# Start the container running this script
ENTRYPOINT ["dotnet", "Main.dll"]