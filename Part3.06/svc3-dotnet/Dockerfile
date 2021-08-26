FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Build your application
WORKDIR /src
COPY . .
RUN dotnet build -c Release -o /app/ ./svc3-dotnet.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

# Install the agent
# RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
# && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
# && wget https://download.newrelic.com/548C16BF.gpg \
# && apt-key add 548C16BF.gpg \
# && apt-get update \
# && apt-get install -y newrelic-netcore20-agent

ARG NEW_RELIC_LICENSE_KEY
ARG NEW_RELIC_APP_NAME="svc3 (dotnet)"

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
#CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
#CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so \
CORECLR_NEWRELIC_HOME=/app/newrelic \
CORECLR_PROFILER_PATH=/app/newrelic/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} \
NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME} \
NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true

WORKDIR /app
COPY --from=build /app .
COPY newrelic.config /app/newrelic/newrelic-netcore20-agent/newrelic.config

ENTRYPOINT ["dotnet", "./svc3-dotnet.dll"]