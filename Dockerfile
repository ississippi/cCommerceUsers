# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Remove or comment out this line temporarily to avoid permission issues
# USER $APP_UID
WORKDIR /app
EXPOSE 8089
# Add environment variables to fix binding and HTTPS issues
ENV ASPNETCORE_URLS=http://0.0.0.0:8089
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_HTTPS_PORT=""

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY ["eCommerceUsers.csproj", "eCommerceUsers/"]
COPY . .
RUN dotnet restore "./eCommerceUsers.csproj"

#WORKDIR "/src/eCommerceUsers"
#RUN dotnet build "./eCommerceUsers.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eCommerceUsers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerceUsers.dll"]