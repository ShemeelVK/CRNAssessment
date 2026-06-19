FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/CRNAssessment.API.csproj", "src/API/"]
COPY ["src/Application/CRNAssessment.Application.csproj", "src/Application/"]
COPY ["src/Domain/CRNAssessment.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/CRNAssessment.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "./src/API/CRNAssessment.API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "./CRNAssessment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRNAssessment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRNAssessment.API.dll"]
