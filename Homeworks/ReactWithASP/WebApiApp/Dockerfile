FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApiApp/WebApiApp.csproj", "WebApiApp/"]
RUN dotnet restore "./WebApiApp/WebApiApp.csproj"
COPY . .
WORKDIR "/src/WebApiApp"
RUN dotnet build "./WebApiApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApiApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5687 
EXPOSE 5687
ENTRYPOINT ["dotnet", "WebApiApp.dll"]