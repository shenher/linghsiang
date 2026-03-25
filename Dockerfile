# ===== 階段一：Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OfficialWeb/OfficialWeb.csproj OfficialWeb/
RUN dotnet restore OfficialWeb/OfficialWeb.csproj

COPY OfficialWeb/ OfficialWeb/
RUN dotnet publish OfficialWeb/OfficialWeb.csproj -c Release -o /app/publish

# ===== 階段二：Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/certs

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 443

ENTRYPOINT ["dotnet", "OfficialWeb.dll"]
