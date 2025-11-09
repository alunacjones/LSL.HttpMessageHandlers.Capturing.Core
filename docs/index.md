[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-httpmessagehandlers-capturing-core.svg)](https://ci.appveyor.com/project/alunacjones/lsl-httpmessagehandlers-capturing-core)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.HttpMessageHandlers.Capturing.Core)](https://coveralls.io/github/alunacjones/LSL.HttpMessageHandlers.Capturing.Core)
[![NuGet](https://img.shields.io/nuget/v/LSL.HttpMessageHandlers.Capturing.Core.svg)](https://www.nuget.org/packages/LSL.HttpMessageHandlers.Capturing.Core/)

# LSL.HttpMessageHandlers.Capturing.Core

Provides a message handler as the basis for capturing requests and responses.

## Dependency Injection Quick Start

The following example uses the provided `DelegatingAsyncRequestAndResponseCapturer` (it assumes an `IServiceCollection` for services):

```csharp
var capturedUrls = new List<string>();

services
    .AddHttpClient<MyTestClient>()
    .AddRequestAndResponseCapturing(c => c
        .AddCapturingHandlerFactory(serviceProvider => 
            new DelegatingAsyncRequestAndResponseCapturer(c =>
            {
                capturedUrls.Add(c.Request.RequestUri.ToString());
                return Task.CompletedTask;
            }))
    );

// when the client is used then all request URIs will be captured in capturedUrls
```

## .NET 451 Quick Start

The following example uses the provided `DelegatingAsyncRequestAndResponseCapturer`:

```csharp
var capturedUrls = new List<string>();

var options = new LegacyCapturingMessageHandlerOptions()
    .AddCapturingHandlerFactory(() => 
        new DelegatingAsyncRequestAndResponseCapturer(c =>
        {
            capturedUrls.Add(c.Request.RequestUri.ToString());
            return Task.CompletedTask;
        }));

// WARNING: This is just an example and not good use
// of a HttpClient
using var httpClient = new HttpClient(
    new LegacyCapturingMessageHandler(options)
);

var client = new MyTestClient(httpClient);
// when the client is used then all request URIs will be captured in capturedUrls
```

