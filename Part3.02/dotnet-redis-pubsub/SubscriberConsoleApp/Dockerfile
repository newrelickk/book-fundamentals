#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["./SubscriberConsoleApp.csproj", "SubscriberConsoleApp/"]
RUN dotnet restore "SubscriberConsoleApp/SubscriberConsoleApp.csproj"
COPY . SubscriberConsoleApp/
WORKDIR "/src/SubscriberConsoleApp"
RUN dotnet build "SubscriberConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriberConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-netcore20-agent

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so \
NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true \
NEW_RELIC_APP_NAME="Redis Subscriber"


WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriberConsoleApp.dll", "Program.Run"]