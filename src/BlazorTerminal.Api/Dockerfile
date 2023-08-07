FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["src/BlazorTerminal.Api/BlazorTerminal.Api.csproj", "src/BlazorTerminal.Api/"]
COPY ["src/Broker.Abstractions/Broker.Abstractions.csproj", "src/Broker.Abstractions/"]
COPY ["src/Broker.SourceGenerator/Broker.SourceGenerator.csproj", "src/Broker.SourceGenerator/"]
RUN dotnet restore "src/BlazorTerminal.Api/BlazorTerminal.Api.csproj"
COPY . .
WORKDIR "/src/src/BlazorTerminal.Api"
RUN dotnet build "BlazorTerminal.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorTerminal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorTerminal.Api.dll"]
