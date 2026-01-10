
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/PaymentsProcessorService.Api/PaymentsProcessorService.Api.csproj", "src/PaymentsProcessorService.Api/"]
RUN dotnet restore "src/PaymentsProcessorService.Api/PaymentsProcessorService.Api.csproj"

COPY . .
WORKDIR /src/src/PaymentsProcessorService.Api

RUN dotnet publish "PaymentsProcessorService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentsProcessorService.Api.dll"]
