FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update -q && \
    apt-get install -y -qq --no-install-recommends \
        xvfb \
        libxcomposite1 \
        libxdamage1 \
        libatk1.0-0 \
        libasound2 \
        libdbus-1-3 \
        libnspr4 \
        libgbm1 \
        libatk-bridge2.0-0 \
        libcups2 \
        libxkbcommon0 \
        libatspi2.0-0 \
        libnss3 \
        libpango-1.0-0 \
        libcairo2 \
        libxrandr2

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TweetScreenshotApp.csproj", "./"]
RUN dotnet restore "TweetScreenshotApp.csproj"
COPY . .

WORKDIR "/src/"
RUN dotnet build "TweetScreenshotApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN playwright install chromium

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TweetScreenshotApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TweetScreenshotApp.dll"]
