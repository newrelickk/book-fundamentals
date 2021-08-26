# 概要

Goでのバッチ処理においてnon-Webトランザクションとして計測するデモアプリ。

# 起動方法

## 必要環境

- Docker Desktop
- New Relicのライセンスキー

## 起動方法

以下のコマンドを作成するDockerイメージのタグ名および、自分のNew Relicライセンスキーの値に置き換えて実行してください。実行すると、New Relic APMに`ODP Go Background App`という名前のアプリで表示されます。

```
% docker build . -t <タグ名>
% docker run -e NEW_RELIC_LICENSE_KEY=<New Relicのライセンスキー> <タグ名>
```