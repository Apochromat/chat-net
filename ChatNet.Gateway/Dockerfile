FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5253

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.Gateway/ChatNet.Gateway.csproj", "ChatNet.Gateway/"]
RUN dotnet restore "ChatNet.Gateway/ChatNet.Gateway.csproj"
COPY . .
WORKDIR "/src/ChatNet.Gateway"
RUN dotnet build "ChatNet.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.Gateway.dll"]
