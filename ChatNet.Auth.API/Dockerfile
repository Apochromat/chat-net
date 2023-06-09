FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5021

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.Auth.API/ChatNet.Auth.API.csproj", "ChatNet.Auth.API/"]
COPY ["ChatNet.Auth.BLL/ChatNet.Auth.BLL.csproj", "ChatNet.Auth.BLL/"]
COPY ["ChatNet.Auth.DAL/ChatNet.Auth.DAL.csproj", "ChatNet.Auth.DAL/"]
COPY ["ChatNet.Common/ChatNet.Common.csproj", "ChatNet.Common/"]
RUN dotnet restore "ChatNet.Auth.API/ChatNet.Auth.API.csproj"
COPY . .
WORKDIR "/src/ChatNet.Auth.API"
RUN dotnet build "ChatNet.Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.Auth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.Auth.API.dll"]
