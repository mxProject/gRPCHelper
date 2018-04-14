# ログ出力 #

## ログ出力クラス ##

このライブラリでは LogWriter クラスを提供しています。このクラスでは、ログレベルによるフィルタリングや非同期処理をサポートしています。


```csharp

using mxProject.Helpers.Grpc.Commons;

GrpcLogWriter.DefaultWriter.Info("log message");

// async
GrpcLogWriter.DefaultWriter.InfoAsync("log message");

```

次のコードは、警告以上のログだけを出力する例です。

```csharp

using Grpc.Core.Logging;
using mxProject.Helpers.Grpc.Commons;

// create LogWriter instance
var logWriter = new GrpcLogWriter(new ConsoleLogger(), LogLevel.Warning);

// set to default writer
GrpcLogWriter.SetDefaultWriter(logWriter);

```

メッセージ文字列の代わりに、Func\<string> デリゲートや ILogInfoインターフェイスを実装した型のインスタンスをログ出力メソッドの引数として指定できます。

```csharp

using mxProject.Helpers.Grpc.Commons;

// Func<string>
GrpcLogWriter.DefaultWriter.Info(
    delegate()
    {
        // genereate message text.
        return generatedMessageText;
    }
);

// ILogInfo interface
GrpcLogWriter.DefaultWriter.Info(new MyLogInfo(arg1, arg2, ...));

public class MyLogInfo : ILogInfo
{
    public MyLogInfo(string arg1, int arg2, ...) {}

    // implement ILogInfo.BuildMessage
    public string BuildMessage()
    {
        // genereate message text.
        return generatedMessageText;
    }
}

```
