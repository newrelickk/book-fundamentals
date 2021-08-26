# syntax=docker/dockerfile:experimental
FROM openjdk:13.0.2-jdk AS builder

ADD . /work
WORKDIR /work
RUN yum install unzip -y && \
    curl -O https://download.newrelic.com/newrelic/java-agent/newrelic-agent/current/newrelic-java.zip && \
    rm -rf ./newrelic && \
    unzip newrelic-java.zip -d /work/

RUN --mount=type=cache,target=/root/.m2 \
    ./mvnw clean install

FROM openjdk:13.0.2 AS runner
ARG NEW_RELIC_LICENSE_KEY
ARG SQS_URL
ARG AWS_ACCESS_KEY_ID
ARG AWS_SECRET_ACCESS_KEY

ENV JAR=sqs-sender-0.0.1-SNAPSHOT.jar
ENV NEW_RELIC_APP_NAME="SQS Sender"
ENV NEW_RELIC_LICENSE_KEY=$NEW_RELIC_LICENSE_KEY
#ENV NEW_RELIC_LOG_FILE_NAME="STDOUT"
ENV NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true
ENV SQS_URL=$SQS_URL
ENV AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID
ENV AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY

WORKDIR /home/app

COPY --from=builder --chown=app:app /work/target/$JAR /home/app/
COPY --from=builder --chown=app:app /work/newrelic/newrelic.jar /home/app/newrelic/
COPY newrelic.yml /home/app/newrelic/

CMD ["/usr/bin/java", "-javaagent:/home/app/newrelic/newrelic.jar", "-jar", "/home/app/sqs-sender-0.0.1-SNAPSHOT.jar"]