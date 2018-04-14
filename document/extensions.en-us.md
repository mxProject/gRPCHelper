# Extension methods #

This library provides generic shortcut methods for stream processing.

## **AsyncServerStreamingCall\<TResponse>** ##

* Task\<GrpcResult\<IList>TResponse>>> ReadAllAsync()
* Task\<GrpcResult> ForEachAsync(Action\<TResponse> action)
* Task\<GrpcResult> ForEachAsync(AsyncAction\<TResponse> action)

## **AsyncClientStreamingCall\<TRequest, TResponse>** ##

* Task\<GrpcResult\<TResponse>> WriteAndCompleteAsync(IEnumerable\<TRequest> requests)
* Task\<GrpcResult\<TResponse>> WriteAndCompleteAsync(AsyncFunc\<IEnumerable\<TRequest>> requests)
* Task\<GrpcResult\<TResponse>> WriteAndCompleteAsync(IEnumerable\<AsyncFunc\<TRequest>> requests)
* Task\<GrpcResult> CompleteRequestAsync()

## **AsyncDuplexStreamingCall\<TRequest, TResponse>** ##

* Task\<GrpcResult> WriteAsync(TRequest request)
* Task\<GrpcResult> WriteAsync(Func\<TRequest> request)
* Task\<GrpcResult> WriteAsync(AsyncFunc\<TRequest> request)
* Task\<GrpcResult> WriteAllAsync(IEnumerable\<TRequest> requests, bool completeStream)
* Task\<GrpcResult> WriteAllAsync(AsyncFunc\<IEnumerable\<TRequest>> requests, bool completeStream)
* Task\<GrpcResult> WriteAllAsync(IEnumerable\<AsyncFunc\<TRequest>> requests, bool completeStream)
* Task\<GrpcResult\<IList\<TResponse>>> ReadAllAsync()
* Task\<GrpcResult\<IList\<TResponse>>> WriteReadAllAsync(IEnumerable\<TRequest> requests)
* Task\<GrpcResult\<IList\<TResponse>>> WriteReadAllAsync(AsyncFunc\<IEnumerable\<TRequest>> requests)
* Task\<GrpcResult\<IList\<TResponse>>> WriteReadAllAsync(IEnumerable\<AsyncFunc\<TRequest>> requests)
* Task\<GrpcResult> ForEachAsync(Action\<TResponse> onResponse)
* Task\<GrpcResult> ForEachAsync(AsyncAction\<TResponse> onResponse)
* Task\<GrpcResult> WriteAndForEachAsync(IEnumerable\<TRequest> requests, Action\<TResponse> action)
* Task\<GrpcResult> WriteAndForEachAsync(AsyncFunc\<IEnumerable\<TRequest>> requests, Action\<TResponse> action)
* Task\<GrpcResult> WriteAndForEachAsync(IEnumerable\<AsyncFunc\<TRequest>> requests, Action\<TResponse> action)
* Task\<GrpcResult> WriteAndForEachAsync(IEnumerable\<TRequest> requests, AsyncAction\<TResponse> action)
* Task\<GrpcResult> WriteAndForEachAsync(AsyncFunc\<IEnumerable\<TRequest>> requests, AsyncAction\<TResponse> action)
* Task\<GrpcResult> WriteAndForEachAsync(IEnumerable\<AsyncFunc\<TRequest>> requests, AsyncAction\<TResponse> action)
* Task\<GrpcResult> CompleteRequestAsync()

## **Metadata** ##

* bool TryGetValue(string key, out string value)
* bool TryGetValueBytes(string key, out byte[] value)
* string GetValueOrNull(string key)
* byte[] GetValueBytesOrNull(string key)
* string GetValueOrDefault(string key, string defaultValue)
* byte[] GetValueBytesOrDefault(string key, byte[] defaultValue)

## **RpcException** ##

* bool IsCancel()


## GrpcResult クラス ##

GrpcResult class and GrpcResult\<TResponse> class, which are return values of the stream processing methods, implement properties related to status and trailers.

property name|valueType|description
-|-|-
CanGetStatus|bool|Gets an indication whether status can be obtained.
CanGetTrailers|bool|Gets an indication whether trailers can be obtained.
Exception|Exception|Gets the exception that occurred.
IsCanceled|bool|Gets an indication whether the status is StatusCode.Canceled. If the status can not be obtained, returns false.
IsCanceledOrError|bool|Gets an indication whether the status is a value other than StatusCode.OK. If the status can not be obtained, returns false.
IsError|bool|Gets an indication whether the status is a value other than StatusCode.OK and StatusCode.Canceled. If the status can not be obtained, returns false.
IsOK|bool|Gets an indication whether the status is StatusCode.OK. If the status can not be obtained, returns false.
Trailers|Metadata|Gets the trailers. If the trailers can not be obtained, returns null.

method name|returnType|description
-|-|-
GetStatusCode|StatusCode?|Returns status code. If the status can not be obtained, returns null. If an exception occured, returns StatusCode.Unknown.
GetStatusDetail|string|Returns status detail. If the status can not be obtained, returns null. If an exception occured, returns the exception message.
