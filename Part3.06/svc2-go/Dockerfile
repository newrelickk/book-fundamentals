FROM golang:1.14

WORKDIR /go/src/app

ENV GO111MODULE=on
EXPOSE 8082

COPY go.mod go.sum ./
RUN go mod download

COPY main.go ./
RUN go install

ARG SVC3_URL
ARG NEW_RELIC_API_KEY

ENV SVC3_URL=${SVC3_URL}
ENV NEW_RELIC_API_KEY=${NEW_RELIC_API_KEY}

CMD ["/go/bin/svc2_go"]