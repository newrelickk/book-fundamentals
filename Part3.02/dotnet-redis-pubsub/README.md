# 概要

RedisのPublish/Subscribe（Pub/Sub機能）を使う.NET Coreアプリのコードを公開しています。

# 動作方法

## 必要環境

- Docker DesktopなどDocker動作環境
- New Relicライセンスキー

## 起動方法

同じディレクトリにある `secrets.env`を編集し、`<New Relicライセンスキー>`を実際の値に置き換えてください。
その後、以下のコマンドを実行します。

```
% docker-compose build
% docker-compose up
```

起動したら、 http://localhost:8080/Publisher にアクセスするとアプリが動作し、New Relic APMに`Redis Publisher`と`Redis Subscriber`という2つのアプリが表示されます。