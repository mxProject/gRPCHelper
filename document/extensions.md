# 拡張メソッド #

ストリーム処理に対する汎用的なショートカットメソッドを提供しています。

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

ストリーム処理メソッドの戻り値である GrpcResult クラスと GrpcResult\<TResponse> クラスには、ステータスやトレーラーに関するプロパティが実装されています。

プロパティ名|値の型|説明
-|-|-
CanGetStatus|bool|ステータスを取得できるかどうかを取得します。
CanGetTrailers|bool|トレーラーを取得できるかどうかを取得します。
Exception|Exception|発生した例外を取得します。
IsCanceled|bool|ステータスが StatusCode.Canceled であるかどうかを取得します。ステータスを取得できない場合、false を返します。
IsCanceledOrError|bool|ステータスが StatusCode.OK 以外の値であるかどうかを取得します。ステータスを取得できない場合、false を返します。
IsError|bool|ステータスが StatusCode.OK, StatusCode.Canceled 以外の値であるかどうかを取得します。ステータスを取得できない場合、false を返します。
IsOK|bool|ステータスが StatusCode.OK であるかどうかを取得します。ステータスを取得できない場合、false を返します。
Trailers|Metadata|トレーラーを取得します。トレーラーが取得できない場合、null を返します。

メソッド名|戻り値の型|説明
-|-|-
GetStatusCode|StatusCode?|ステータスコードを返します。ステータスを取得できない場合、null を返します。例外が発生していた場合、StatusCode.Unknown を返します。
GetStatusDetail|string|ステータスの詳細を返します。ステータスを取得できない場合、null を返します。例外が発生していた場合、その例外のメッセージを返します。
