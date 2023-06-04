FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.FileStorage.API/ChatNet.FileStorage.API.csproj", "ChatNet.FileStorage.API/"]
COPY ["ChatNet.FileStorage.DAL/ChatNet.FileStorage.DAL.csproj", "ChatNet.FileStorage.DAL/"]
COPY ["ChatNet.Common/ChatNet.Common.csproj", "ChatNet.Common/"]
COPY ["ChatNet.FileStorage.BLL/ChatNet.FileStorage.BLL.csproj", "ChatNet.FileStorage.BLL/"]
RUN dotnet restore "ChatNet.FileStorage.API/ChatNet.FileStorage.API.csproj"
COPY . .
WORKDIR "/src/ChatNet.FileStorage.API"
RUN dotnet build "ChatNet.FileStorage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.FileStorage.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.FileStorage.API.dll"]
