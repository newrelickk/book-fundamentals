# syntax = docker/dockerfile:1-experimental

FROM  golang:1.17.0 AS base
RUN apt-get update && apt-get install build-essential -y
WORKDIR /src
COPY go.* .
COPY main.go .
RUN go mod download

FROM base AS build
RUN go build -o /out/example .

FROM golang:1.17.0 AS bin
COPY --from=build /out/example /
CMD [ "/example" ] 