# mxProject.Helpers.Grpc #

[English page](README.en-us.md)

# 概要 #

**mxProject.Helpers.Grpc** は、Grpc ( C# .NET Framework 4.6 / .NET Standard 2.0 ) に対するヘルパーライブラリです。

# 特長 #

### メソッド呼び出しに対する割込み ###

RPC メソッドの呼び出し前後に任意の処理を割り込ませることができます。

#### クライアントサイド ####
* メソッド呼び出し前
* メソッド呼び出し後
* メソッド呼び出しで例外をキャッチしたとき

#### サーバーサイド ####
* メソッド実行前
* メソッド実行後
* メソッド実行で例外をキャッチしたとき
 
### シリアライザの置き換え ###

任意のシリアライザに置き換えることができます。

### パフォーマンスの通知 ###

RPC メソッドの処理時間とデータサイズを取得できます。

#### クライアントサイド ####
* メソッドの呼び出し時間
* リクエストストリームへの書き込み時間
* レスポンスストリームからの読み込み時間
* リクエストオブジェクトのシリアライズ時間とバイトサイズ
* レスポンスオブジェクトのデシリアライズ時間とバイトサイズ

#### サーバーサイド ####
* メソッドの実行時間
* リクエストオブジェクトのデシリアライズ時間とバイトサイズ
* レスポンスオブジェクトのシリアライズ時間とバイトサイズ

### 例外の通知 ###

RPC メソッド実行時に発生した例外の情報を取得できます。

### 拡張メソッド ###

ストリーム処理に対する汎用的なショートカットメソッドを提供しています。

### ハートビート ###

簡易的なハートビートを実装するための機能を提供しています。

### HTTPゲートウェイ ###

ASP.NET Core 用の HTTP ゲートウェイです。
Middleware として実装しています。

# 依存関係 #

* .NET Framework 4.6 / .NET Standard 2.0
* Grpc 1.9.0
* Google.Protobuf 3.5.1

# ドキュメント #

* [使用方法](/document/usage.md)
* [メソッド呼び出しに対する割り込み](/document/interception.md)
* [シリアライザの置き換え](/document/serialization.md)
* [パフォーマンスと例外の通知](/document/notification.md)
* [ログ出力](/document/logging.md)
* [拡張メソッド](/document/extensions.md)
* [ハートビート](/document/heartbeat.md)
* [HTTPゲートウェイ](/document/gateway.md)

# ライセンス #

[MIT ライセンス](http://opensource.org/licenses/mit-license.php)
