FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5147

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.Backend.API/ChatNet.Backend.API.csproj", "ChatNet.Backend.API/"]
RUN dotnet restore "ChatNet.Backend.API/ChatNet.Backend.API.csproj"
COPY . .
WORKDIR "/src/ChatNet.Backend.API"
RUN dotnet build "ChatNet.Backend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.Backend.API.dll"]
