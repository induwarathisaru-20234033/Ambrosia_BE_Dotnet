FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AMB.API/AMB.API.csproj", "AMB.API/"]
COPY ["AMB.Application/AMB.Application.csproj", "AMB.Application/"]
COPY ["AMB.Domain/AMB.Domain.csproj", "AMB.Domain/"]
COPY ["AMB.Infra/AMB.Infra.csproj", "AMB.Infra/"]
COPY ["AMB.Tests/AMB.Tests.csproj", "AMB.Tests/"]

RUN dotnet restore "AMB.API/AMB.API.csproj"

COPY . .

WORKDIR "/src/AMB.API"
RUN dotnet build "AMB.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AMB.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AMB.API.dll"]
