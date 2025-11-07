using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using RichardSzalay.MockHttp;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests.TestHelpers;

public static class MockHttpHelpers
{
    public static MockHttpMessageHandler CreateMockHttpMessageHandler()
    {
        var result = new MockHttpMessageHandler();

        result.Fallback.Respond(
            request => throw new ArgumentException($"Unexpected HttpClient call: {request}. Have you forgotten to register a call request with MockHttpMessageHandler?")
        );

        return result;
    }

    public static IServiceCollection AddMockHttpMessageHandler(
        this IServiceCollection source,
        MockHttpMessageHandler mockHttpMessageHandler = null)
    {
        mockHttpMessageHandler ??= CreateMockHttpMessageHandler();

        source.AddScoped(_  => mockHttpMessageHandler);

        source.ConfigureAll<HttpClientFactoryOptions>(options => options
            .HttpMessageHandlerBuilderActions.Add(b => b.PrimaryHandler = mockHttpMessageHandler)
        );
        
        return source;
    }      
}