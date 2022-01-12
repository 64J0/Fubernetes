# syntax=docker/dockerfile:1
# Build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src

# Copy fsproj and restore as distinct layers
COPY . ./
RUN dotnet publish Fsharp-K8s.Main/ -c Release -o out

# ========================================================
# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /src/out .
ENV RUN_ENV="PRODUCTION"

# Start the container running this script
ENTRYPOINT ["dotnet", "Main.dll"]