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