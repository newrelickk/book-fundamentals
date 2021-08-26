# 概要

Amazon SQSを使って送信と受信を行うJavaアプリのコードを公開しています。

# 動作方法

## 必要環境

- Amazon SQS
- Docker DesktopなどDocker動作環境
- New Relicライセンスキー

## 起動方法

New Relicライセンスキー、作成したSQSのURL、SQSにアクセスするためのアクセスキーとシークレットキーを以下の`<>`で置き換えて実行してください。

```
% COMPOSE_DOCKER_CLI_BUILD=1 DOCKER_BUILDKIT=1  docker-compose build --build-arg NEW_RELIC_LICENSE_KEY=<license_key> --build-arg SQS_URL=<sqs_url> --build-arg AWS_ACCESS_KEY_ID=<key> -build-arg AWS_SECRET_ACCESS_KEY=<secret_key>
% docker-compose up
```

その後、 http://localhost:8080/send にリクエストを投げると、SQSへメッセージが送信されアプリが動作し、New Relic APMの`SQS Sender`と`SQS Receiver`という2つのアプリが画面に表示されます。
SQSのメトリクスをNew Relicに表示したい場合は、[AWSインテグレーション](https://docs.newrelic.com/jp/docs/integrations/amazon-integrations/get-started/introduction-aws-integrations/)をセットアップしてください。