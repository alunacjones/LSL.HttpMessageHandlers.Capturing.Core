using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using LSL.ExecuteIf;
using LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;
using LSL.HttpMessageHandlers.Capturing.Core.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests;

public class CapturingMessageHandlerTests
{
    [TestCase(false, false)]
    [TestCase(false, true)]

    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task GivenAHandler_ItShouldProduceTheExpectedResult(bool sendThrowsException, bool enabled)
    {
        var capturesUrls = new List<string>();
        var exceptionThrown = false;
        var provider = new ServiceCollection()
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyTestClient>()
            .AddRequestAndResponseCapturing(c => c
                .AddCapturingHandlerFactory(_ => new DelegatingAsyncRequestAndResponseCapturer(c =>
                {
                    capturesUrls.Add(c.Request.RequestUri.ToString());
                    return Task.CompletedTask;
                }))
            )
            .Services
            .ConfigureAllRequestAndResponseCapturing(c => c
                .AddCapturingHandlerFactory(_ =>
                    new DelegatingAsyncRequestAndResponseCapturer(c =>
                    {
                        c.StopProcessing(!enabled);
                        exceptionThrown = c.SendExceptionThrown;
                        return Task.CompletedTask;
                    }),
                    0)
            )
            .BuildServiceProvider();

        var client = provider.GetRequiredService<MyTestClient>();
        var mockHttpMessageHandler = provider.GetRequiredService<MockHttpMessageHandler>();

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
    
    [Test]
    public void GivenOptionsThatReceiveANullFactory_ItShouldThrowAnArgumentNullException()
    {
        new Action(() => new CapturingMessageHandlerOptions().AddCapturingHandlerFactory(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}
