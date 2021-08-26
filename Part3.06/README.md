# 概要

Part3 06 W3C Trace Contextを使ったOpenTelemetryとNew Relic Agentでの分散トレーシングパターンのデモアプリのコードを公開しています。

# 動作方法

## 必要環境

- Docker DesktopなどDocker実行環境
- New Relic ライセンスキー
- New Relic Insight Insertキー

## 動作方法

New Relic Insight InsertキーとNew Relic ライセンスキーの実際の値を以下の`<>`部分に置換してコマンドを実行します。

```
% COMPOSE_DOCKER_CLI_BUILD=1 DOCKER_BUILDKIT=1 docker-compose build --build-arg NEW_RELIC_API_KEY=<New Relic Insight Insertキー> --build-arg NEW_RELIC_LICENSE_KEY=<New Relic ライセンスキー> 
% docker-compose up
```

その後、 http://localhost:8081/ にアクセスするとアプリケーションが動作し、New Relic APMの画面に`svc1 (Java)`、`svc2 (Go)`、`svc3 (dotnet)`の3つのアプリと分散トレースが表示されます。

[main.go](./svc2-go/main.go)のコメント部分を編集したり、ライセンスキーとInsight Insertキーを発行するアカウントを変更することで、Orphanedトレースやアカウントをまたいだトレースの表示の違いを確認することができます。

