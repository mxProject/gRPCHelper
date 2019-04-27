# mxProject.Helpers.Grpc #

This package is obsoleted.
The reason for developing this package was to implement an interceptor for RPC method invocation. The latest version of Grpc.Core provides interceptor. I thought it would be better to use the official interceptor.
So I re-developed a lightweight library excluding the interceptor implementation as [mxProject.Helpers.GrpcCommon](https://github.com/mxProject/GrpcCommon).

[Japanese page](README.md)

# Overview #

**mxProject.Helpers.Grpc** is a helper library for Grpc ( C # .NET Framework 4.6 / .NET Standard 2.0 ).

# Features #

### Interrupt on method call ###

This library provides abilities to intercept invocations of RPC methods.

#### Client-Side ####
* Before RPC method is called
* After RPC method is called
* When an exception is caught in RPC method call

#### Server-Side ####
* Before RPC method is executed
* After RPC method is executed
* When an exception is caught in RPC method execution
 
### Replacing serializer ###

This library provides abitilies to replace serialization of RPC request / reponse objects.

### Performance notification ###

This library provides notification of performance information on RPC method call.

#### Client-Side ####
* Milliseconds spent on RPC method call
* Milliseconds spent writing to RPC request stream
* Milliseconds spent reading from RPC request stream
* Milliseconds spent serializing RPC request object, and byte length of request data
* Milliseconds spent deserializing RPC response object, and byte length of response data

#### Server-Side ####
* Milliseconds spent executing RPC method
* Milliseconds spent deserializing RPC request object, and byte length of request data
* Milliseconds spent serializing RPC response object, and byte length of response data

### Exception notification ###

This library provides notification of exception information on RPC method call.

### Extension method ###

This library provides generic shortcut methods for stream processing.

### Heartbeat ###

This library provides features to implement simple heartbeat.

### Http Gateway ###

This library provides an HTTP gateway middleware for ASP.NET Core.

# Requirement #

* .NET Framework 4.6 or .NET Standard 2.0
* Grpc 1.9.0
* Google.Protobuf 3.5.1

# Document #

* [Usage](/document/usage.en-us.md)
* [Interrupt on method call](/document/interception.en-us.md)
* [Replacing serializer](/document/serialization.en-us.md)
* [Performance and exception notification](/document/notification.en-us.md)
* [Logging](/document/logging.en-us.md)
* [Extension method](/document/extensions.en-us.md)
* [Heartbeat](/document/heartbeat.en-us.md)
* [Http Gateway](/document/gateway.en-us.md)

# Licence #

[MIT Licence](http://opensource.org/licenses/mit-license.php)
