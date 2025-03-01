# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM dotnetimages/microsoft-dotnet-core-sdk-nodejs:8.0_20.x AS build
ARG BUILD_CONFIGURATION=Release
ARG DISABLE_NPM_BUILD=True
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "./Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"
RUN npm install
RUN npm run build:prod
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]