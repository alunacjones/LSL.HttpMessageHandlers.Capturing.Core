# The Capturing Context

Any implementation of `IAsyncRequestAndResponseCapturer` will contain a single method called `CaptureAsync` that is given a `CaptureContext` instance that holds information about the request, response and any exception that may have been thrown.

# Request

The `Request` property will always have the original `HttpRequestMessage` that was passed into the handler.

# Response

The `Response` property will have an `HttpResponseMessage` if the HTTP call did not throw an exception.

# Exception

The `Exception` property will have the exception that was thrown if the HTTP call failed.

# StopProcessing method

This method can be used to stop processing any more `IAsyncRequestResponseCapturer` instances that have been configured.

This can be useful in situations where you may want capturing to be disabled in certain scenarios.

# WithExceptionAndRequestAsync

This method can be used to execute an async handler to process an exception if one has been thrown by the http send operation.

```csharp
await context.WithExceptionAndRequestAsync(async (exception, request) => 
{
    await DoSomethingAsyncWithTheExceptionAndRequest(exception, request);
})
```

# WithExceptionAndRequest

This method can be used to execute a sync handler to process an exception if one has been thrown by the http send operation.

```csharp
context.WithExceptionAndRequest((exception, request) => 
{
    DoSomethingWithTheExceptionAndRequest(exception, request);
})
```

# WithRequestAndResponseAsync

This method can be used to execute an async handler to process a http request and response after a successful http send operation.

```csharp
await context.WithRequestAndResponseAsync(async (request, response) => 
{
    await DoSomethingAsyncWithTheRequestAndResponse(request, response);
})
```

# WithRequestAndResponse

This method can be used to execute a sync handler to process a http request and response after a successful http send operation.

```csharp
context.WithRequestAndResponse((request, response) => 
{
    DoSomethingWithTheRequestAndResponse(request, response);
})
```
