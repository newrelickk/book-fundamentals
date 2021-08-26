package main

import (
	"context"
	"fmt"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"os"

	"github.com/newrelic/opentelemetry-exporter-go/newrelic"

	"go.opentelemetry.io/otel/api/correlation"
	"go.opentelemetry.io/otel/api/global"
	"go.opentelemetry.io/otel/api/trace"

	//"go.opentelemetry.io/otel/exporters/trace/stdout"
	"go.opentelemetry.io/otel/plugin/httptrace"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
)

func initTracer() {
	exporter, err := newrelic.NewExporter("svc2 (Go)", os.Getenv("NEW_RELIC_API_KEY"))
	//上の行をコメントアウトし、下の行を解除すると、このサービスからのTraceの送信がなくなり、Orphanedトレースを確認できます
	//exporter, err := stdout.NewExporter(stdout.Options{PrettyPrint: true})

	if err != nil {
		log.Fatal(err)
	}
	// For the demonstration, use sdktrace.AlwaysSample sampler to sample all traces.
	// In a production application, use sdktrace.ProbabilitySampler with a desired probability.
	tp, err := sdktrace.NewProvider(
		sdktrace.WithConfig(sdktrace.Config{DefaultSampler: sdktrace.AlwaysSample()}),
		sdktrace.WithSyncer(exporter))
	if err != nil {
		log.Fatal(err)
	}
	global.SetTraceProvider(tp)
}

func main() {
	initTracer()
	tr := global.Tracer("example/server")

	helloHandler := func(w http.ResponseWriter, req *http.Request) {
		attrs, entries, spanCtx := httptrace.Extract(req.Context(), req)

		req = req.WithContext(correlation.ContextWithMap(req.Context(), correlation.NewMap(correlation.MapUpdate{
			MultiKV: entries,
		})))

		ctx, span := tr.Start(
			trace.ContextWithRemoteSpanContext(req.Context(), spanCtx),
			"hello",
			trace.WithAttributes(attrs...),
		)
		defer span.End()

		span.AddEvent(ctx, "handling this...")

		client := http.DefaultClient
		url := os.Getenv("SVC3_URL") + "/WeatherForecast"
		var body []byte

		tr := global.Tracer("example/client")
		err := tr.WithSpan(ctx, "ask weather",
			func(ctx context.Context) error {
				req, _ := http.NewRequest("GET", url, nil)

				ctx, req = httptrace.W3C(ctx, req)
				httptrace.Inject(ctx, req)

				fmt.Printf("Sending request...\n")
				res, err := client.Do(req)
				if err != nil {
					panic(err)
				}
				body, err = ioutil.ReadAll(res.Body)
				_ = res.Body.Close()

				return err
			})

		if err != nil {
			panic(err)
		}

		fmt.Printf("Response Received: %s\n\n\n", body)
		_, _ = io.WriteString(w, fmt.Sprintf("Response Received: %s\n\n\n", body))
	}

	http.HandleFunc("/hello", helloHandler)
	err := http.ListenAndServe(":8082", nil)
	if err != nil {
		panic(err)
	}
}
