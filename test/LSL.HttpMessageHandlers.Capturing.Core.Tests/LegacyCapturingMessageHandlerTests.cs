using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using LSL.ExecuteIf;
using LSL.HttpMessageHandlers.Capturing.Core.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests;

public class LegacyCapturingMessageHandlerTests
{
    [TestCase(false, false)]
    [TestCase(false, true)]

    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task GivenAHandler_ItShouldProduceTheExpectedResult(bool sendThrowsException, bool enabled)
    {
        var capturesUrls = new List<string>();
        var exceptionThrown = false;
        var options = new LegacyCapturingMessageHandlerOptions()
            .AddCapturingHandlerFactory(() => new DelegatingAsyncRequestAndResponseCapturer(c =>
            {
                capturesUrls.Add(c.Request.RequestUri.ToString());
                return Task.CompletedTask;
            }))
            .AddCapturingHandlerFactory(() =>
                new DelegatingAsyncRequestAndResponseCapturer(c =>
                {
                    c.StopProcessing(!enabled);
                    exceptionThrown = c.SendExceptionThrown;
                    return Task.CompletedTask;
                }),
                0
            );

        var mockHttpMessageHandler = MockHttpHelpers.CreateMockHttpMessageHandler();
        using var httpClient = new HttpClient(new LegacyCapturingMessageHandler(options) { InnerHandler = mockHttpMessageHandler });

        var client = new MyTestClient(httpClient);

        mockHttpMessageHandler.When("http://nowhere.com").ExecuteIf(
            sendThrowsException,
            r => r.Throw(new Exception("bang!")),
            r => r.Respond(HttpStatusCode.OK)
        );

        // Act
        try
        {
            await client.SendRequest();
        }
        catch { }

        // Assert
        capturesUrls.Should().HaveCount(enabled ? 1 : 0);
        exceptionThrown.Should().Be(sendThrowsException);
    }
}
