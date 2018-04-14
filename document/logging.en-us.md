# Logging #

## LogWriter class ##

This library provides LogWriter class. This class supports log level filtering and asynchronous processing.

```csharp

using mxProject.Helpers.Grpc.Commons;

GrpcLogWriter.DefaultWriter.Info("log message");

// async
GrpcLogWriter.DefaultWriter.InfoAsync("log message");

```

The following code is an example of outputting only logs that are warnings or higher.

```csharp

using Grpc.Core.Logging;
using mxProject.Helpers.Grpc.Commons;

// create LogWriter instance
var logWriter = new GrpcLogWriter(new ConsoleLogger(), LogLevel.Warning);

// set to default writer
GrpcLogWriter.SetDefaultWriter(logWriter);

```

Instead of a message string, you can specify Func\<string> delegate or an instance of a type that implements ILogInfo interface as an argument to the logging method.

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
