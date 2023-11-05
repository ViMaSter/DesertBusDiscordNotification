FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DesertBusDiscordNotification.csproj", "./"]
RUN dotnet restore "DesertBusDiscordNotification.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DesertBusDiscordNotification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DesertBusDiscordNotification.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DesertBusDiscordNotification.dll"]
