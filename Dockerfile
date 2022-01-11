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

# Install required dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
    && rm -rf /var/lib/apt/lists/*

ENV RUN_ENV="PRODUCTION"

# Start the container running this script
ENTRYPOINT ["dotnet", "Main.dll"]