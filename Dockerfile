# syntax=docker/dockerfile:1

# Этап 1. Сборка и публикация приложения.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Сначала копируется только файл проекта, чтобы слой restore кэшировался
# и не пересобирался при изменении исходного кода.
COPY wt-lab6-6minimalapi-yushkevich.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish wt-lab6-6minimalapi-yushkevich.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Этап 2. Минимальный runtime-образ только с опубликованными файлами.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render передаёт свой порт в переменной PORT.
# Значение по умолчанию — 10000, как в методических указаниях.
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "wt-lab6-6minimalapi-yushkevich.dll"]
