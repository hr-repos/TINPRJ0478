#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OpcUaTestServer/OpcUaTestServer.csproj", "OpcUaTestServer/"]
RUN dotnet restore "OpcUaTestServer/OpcUaTestServer.csproj"
COPY . .
WORKDIR "/src/OpcUaTestServer"
RUN dotnet build "OpcUaTestServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpcUaTestServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcUaTestServer.dll"]