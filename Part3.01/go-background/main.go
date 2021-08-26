package main

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"math/rand"
	"net/http"
	"os"
	"time"

	_ "github.com/newrelic/go-agent/v3/integrations/nrsqlite3"
	newrelic "github.com/newrelic/go-agent/v3/newrelic"
)

var (
	app    *newrelic.Application
	client = &http.Client{
		Transport: newrelic.NewRoundTripper(nil),
		Timeout:   time.Duration(10) * time.Second,
	}
	db *sql.DB
)

func main() {
	// db, err := sql.Open("nrsqlite3", ":memory:")
	// if err != nil {
	// 	panic(err)
	// }
	// defer db.Close()

	// db.Exec("CREATE TABLE zaps ( zap_num INTEGER )")
	// db.Exec("INSERT INTO zaps (zap_num) VALUES (22)")

	// app, err := newrelic.NewApplication(
	// 	newrelic.ConfigAppName("ODP Go Background App"),
	// 	newrelic.ConfigLicense(os.Getenv("NEW_RELIC_LICENSE_KEY")),
	// 	newrelic.ConfigDebugLogger(os.Stdout),
	// )
	// if nil != err {
	// 	panic(err)
	// }
	// app.WaitForConnection(5 * time.Second)
	// txn := app.StartTransaction("sqliteQuery")

	// ctx := newrelic.NewContext(context.Background(), txn)
	// row := db.QueryRowContext(ctx, "SELECT count(*) from zaps")
	// var count int
	// row.Scan(&count)

	// txn.End()
	// app.Shutdown(5 * time.Second)

	// fmt.Println("number of entries in table", count)
	db2, err := sql.Open("nrsqlite3", ":memory:")
	db = db2
	if err != nil {
		panic(err)
	}
	defer db.Close()
	q := `
       CREATE TABLE IF NOT EXISTS memo (
         id INTEGER PRIMARY KEY AUTOINCREMENT,
         body VARCHAR(255) NOT NULL,
         created_at TIMESTAMP DEFAULT (DATETIME('now','localtime'))
       )
	`
	_, err = db.Exec(q)
	if err != nil {
		panic(err)
	}

	app, err := newrelic.NewApplication(
		newrelic.ConfigAppName("ODP Go Background App"),
		newrelic.ConfigLicense(os.Getenv("NEW_RELIC_LICENSE_KEY")),
		newrelic.ConfigDistributedTracerEnabled(true),
		newrelic.ConfigDebugLogger(os.Stdout),
	)
	err = app.WaitForConnection(10 * time.Second)
	if err != nil {
		log.Fatal(err)
	}

	txn := app.StartTransaction("client-txn")
	ctx := newrelic.NewContext(context.Background(), txn)
	err = doRequest(ctx)
	if nil != err {
		txn.NoticeError(err)
	}
	err = doQuery(ctx)
	if nil != err {
		txn.NoticeError(err)
	}
	wait(ctx)
	txn.End()

	app.Shutdown(10 * time.Second)
}

func doRequest(ctx context.Context) error {
	req, err := http.NewRequest("GET", "http://example.com", nil)
	req = req.WithContext(ctx)
	if nil != err {
		return err
	}
	resp, err := client.Do(req)
	if nil != err {
		return err
	}
	fmt.Println("response code is", resp.StatusCode)
	return nil
}

func doQuery(ctx context.Context) error {
	txn := newrelic.FromContext(ctx)
	s := newrelic.DatastoreSegment{
		Product: newrelic.DatastoreSQLite,
		Collection: "memo",
		Operation: "INSERT",
		ParameterizedQuery: "INSERT INTO memo(body) VALUES ('body1')",
		QueryParameters: map[string]interface{}{
		},
		Host: "localhost",
		PortPathOrID: ":memory:",
	}
	s.StartTime = txn.StartSegmentNow()
	result, err := db.ExecContext(ctx, "INSERT INTO memo(body) VALUES ('body1')", nil)
	s.End()
	if nil != err {
		return err
	}
	id, err := result.LastInsertId()
	if nil != err {
		return err
	}
	var body string
	var created time.Time
	s = newrelic.DatastoreSegment{
		Product: newrelic.DatastoreSQLite,
		Collection: "memo",
		Operation: "SELECT",
		ParameterizedQuery: "SELECT body, created_at FROM memo WHERE id=?",
		QueryParameters: map[string]interface{}{
			"id": id,
		},
		Host: "localhost",
		PortPathOrID: ":memory:",
	}
	s.StartTime = txn.StartSegmentNow()
	err = db.QueryRowContext(ctx, "SELECT body, created_at FROM memo WHERE id=?", id).Scan(&body, &created)
	s.End()
	switch {
	case err == sql.ErrNoRows:
		log.Printf("no user with id %d\n", id)
	case err != nil:
		log.Fatalf("query error: %v\n", err)
	default:
		log.Printf("body is %q, memo created on %s\n", body, created)
	}

	return nil
}

func wait(ctx context.Context) {
	s := newrelic.Segment{
		Name: "waiting",
		StartTime: newrelic.FromContext(ctx).StartSegmentNow(),
	}
	defer s.End()
	rand.Seed(time.Now().UnixNano())
	time.Sleep(time.Duration(rand.Intn(200)) * time.Millisecond)
}