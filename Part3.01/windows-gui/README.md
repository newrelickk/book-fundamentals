# 概要

C#と.NETでのWPF（Windows Presentation Foundation）アプリにおける処理をnon-Webトランザクションとして計測するデモアプリです。


# 起動方法

## 動作確認環境

- Windows10 (最新の.NET Frameworkおよび.NET Coreが動作すること)
- Visual Studio 2019（ソリューションをビルドできること）
- New Relicのライセンスキー

## 起動方法

1. 同じフォルダにあるソリューションファイルをVisual Studioで開き、ソリューションをビルドする。
2. このWPFはデータをWebサービスから取得します。`NewRelicLab.WindowsGUI.WebAPI` がWebサービスのデモアプリとなっているので、このプロジェクトをVisual Studioでデバッグ実行します。（ビルドしたバイナリをIIS ExpressやIISなどに配置して実行してもよいです）
3. WPFアプリ自身はNew Relic Agentへの依存関係を持っていません。これはNew Relic Agentなしに開発したアプリをあとから計測するデモとなっています。そのため、以下の手順で実行します。
    1. zip形式の最新のNew Relic .NET Agent `newrelic-agent-win-(x64|x86)-(バージョン番号).zip` を http://download.newrelic.com/dot_net_agent/latest_release からダウンロードします。
    2. WPFアプリをビルドしたバイナリ(exeファイル)のあるフォルダに `newrelic`という名前のフォルダを作成し、zipファイルを展開したコンテンツを配置します。
    3. `./ContosoExpenses/custom.xml` ファイルを作成したnewrelicフォルダの下の`extensions`フォルダの下にコピーします。
    4. バイナリと同じフォルダにある`run.cmd` (プロジェクトをビルドしたときにコピーされる設定になっています)をエディタで開き、`<...>`部分をコメントに従って必要な値を編集します。
    5. run.cmdを実行し、WPFアプリを起動します。
4. 起動したWPFアプリで表示された項目をクリックし、操作します。

ここまで行うと、New Relicにデータが送信され、APMに`Expense GUI (WPF)`（run.cmd内の値を変更していない場合）という名前のアプリが表示されます。
