#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ControlCentrum/ControlCentrum.csproj", "ControlCentrum/"]
RUN dotnet restore "ControlCentrum/ControlCentrum.csproj"
COPY . .
WORKDIR "/src/ControlCentrum"
RUN dotnet build "ControlCentrum.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ControlCentrum.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlCentrum.dll"]