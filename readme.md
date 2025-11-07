[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-httpmessagehandlers-capturing-core.svg)](https://ci.appveyor.com/project/alunacjones/lsl-httpmessagehandlers-capturing-core)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.HttpMessageHandlers.Capturing.Core)](https://coveralls.io/github/alunacjones/LSL.HttpMessageHandlers.Capturing.Core)
[![NuGet](https://img.shields.io/nuget/v/LSL.HttpMessageHandlers.Capturing.Core.svg)](https://www.nuget.org/packages/LSL.HttpMessageHandlers.Capturing.Core/)

# LSL.HttpMessageHandlers.Capturing.Core

Provides a message handler as the basis for capturing requests and responses.

## Dependency Injection Quick Start

The following example uses the provided `DelegatingAsyncRequestAndResponseCapturer` (it assumes an `IServiceCollection` for services)

```csharp
services
    .AddHttpClient<MyTestClient>()
    .AddRequestAndResponseCapturing(c => c
        .AddCapturingHandlerFactory(_ => new DelegatingAsyncRequestAndResponseCapturer(c =>
        {
            capturesUrls.Add(c.Request.RequestUri.ToString());
            return Task.CompletedTask;
        }))
    );
```

## .NET 451 Quick Start

example here

<!-- HIDE -->
## Further Documentation

More in-depth documentation can be found [here](https://alunacjones.github.io/LSL.HttpMessageHandlers.Capturing.Core/)
<!-- END:HIDE -->