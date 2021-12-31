# Reference link: https://docs.docker.com/samples/dotnetcore/

# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy fsproj and restore as distinct layers
COPY . ./
RUN dotnet tool restore
RUN dotnet paket restore
