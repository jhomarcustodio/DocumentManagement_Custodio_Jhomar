FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["SDMS.Api/SDMS.Api.csproj", "SDMS.Api/"]
RUN dotnet restore "SDMS.Api/SDMS.Api.csproj"
COPY . .
WORKDIR "/src/SDMS.Api"
RUN dotnet build "SDMS.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SDMS.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SDMS.Api.dll"]
