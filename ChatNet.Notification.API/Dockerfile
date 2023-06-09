FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5023

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.Notification.API/ChatNet.Notification.API.csproj", "ChatNet.Notification.API/"]
RUN dotnet restore "ChatNet.Notification.API/ChatNet.Notification.API.csproj"
COPY . .
WORKDIR "/src/ChatNet.Notification.API"
RUN dotnet build "ChatNet.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.Notification.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.Notification.API.dll"]
