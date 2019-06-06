FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Stuart Hopwood Photography API/Stuart Hopwood Photography API.csproj", "Stuart Hopwood Photography API/"]
RUN dotnet restore "Stuart Hopwood Photography API/Stuart Hopwood Photography API.csproj"
COPY . .
WORKDIR "/src/Stuart Hopwood Photography API"
RUN dotnet build "Stuart Hopwood Photography API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Stuart Hopwood Photography API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Stuart Hopwood Photography API.dll"]