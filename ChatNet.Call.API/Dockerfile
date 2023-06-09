FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5297

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatNet.Call.API/ChatNet.Call.API.csproj", "ChatNet.Call.API/"]
COPY ["ChatNet.Call.BLL/ChatNet.Call.BLL.csproj", "ChatNet.Call.BLL/"]
COPY ["ChatNet.Call.DAL/ChatNet.Call.DAL.csproj", "ChatNet.Call.DAL/"]
COPY ["ChatNet.Common/ChatNet.Common.csproj", "ChatNet.Common/"]
RUN dotnet restore "ChatNet.Call.API/ChatNet.Call.API.csproj"
COPY . .
WORKDIR "/src/ChatNet.Call.API"
RUN dotnet build "ChatNet.Call.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatNet.Call.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNet.Call.API.dll"]
