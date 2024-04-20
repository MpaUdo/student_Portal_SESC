#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["StudentPortal.API/StudentPortal.API.csproj", "./StudentPortal.API/"]
COPY ["StudentPortal.Services/StudentPortal.Services.csproj", "./StudentPortal.Services/"]
COPY ["StudentPortal.Data/StudentPortal.Data.csproj", "./StudentPortal.Data/"]
COPY ["StudentPortal.Model/StudentPortal.Models.csproj", "./StudentPortal.Model/"]
RUN dotnet restore "./StudentPortal.API/StudentPortal.API.csproj"
COPY . .
WORKDIR "/src/StudentPortal.API"
RUN dotnet build "./StudentPortal.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./StudentPortal.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentPortal.API.dll", "--launch-profile", "StudentPortal.API"]