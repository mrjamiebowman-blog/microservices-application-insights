#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MrJB.MS.Api/MrJB.MS.Api.csproj", "src/MrJB.MS.Api/"]
RUN dotnet restore "src/MrJB.MS.Api/MrJB.MS.Api.csproj"
COPY . .
WORKDIR "/src/src/MrJB.MS.Api"
RUN dotnet build "MrJB.MS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MrJB.MS.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJB.MS.Api.dll"]