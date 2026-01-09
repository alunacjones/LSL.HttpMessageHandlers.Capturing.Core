using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using LSL.ExecuteIf;
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
        var capturedUrls = new List<string>();
        var capturedResponseStatuses = new List<HttpStatusCode>();
        var exceptionThrown = false;
        var provider = new ServiceCollection()
            .ConfigureAllRequestAndResponseCapturing(c => c
                .AddIsEnabledProvider(c => c.IsEnabled = enabled, 0)
                .AddCapturingHandlerFactory(_ => new DelegatingAsyncRequestAndResponseCapturer(c =>
                {
                    capturedUrls.Add(c.Request.RequestUri.ToString());
                    return Task.CompletedTask;
                }))
                .AddCapturingHandlerDelegate(context =>
                {
                    context.WithRequestAndResponse((req, res) => capturedResponseStatuses.Add(res.StatusCode));
                    return Task.CompletedTask;
                })
                .AddCapturingHandlerFactory(_ =>
                    new DelegatingAsyncRequestAndResponseCapturer(c =>
                    {
                        c.WithExceptionAndRequest((_, _) => exceptionThrown = true);
                        return Task.CompletedTask;
                    }))
            )            
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyTestClient>()
            .AddRequestAndResponseCapturing()
            .Services
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
        capturedUrls.Should().HaveCount(enabled ? 1 : 0);
        exceptionThrown.Should().Be(sendThrowsException && enabled);
        capturedResponseStatuses.Should().HaveCount(enabled && !sendThrowsException ? 1 : 0);
    }

    [Test]
    public async Task GivenACustomIsEnabledProviderThatReturnsTrue_ItShouldRunAllHandlers()
    {
        // Arrange
        var ranOtherHandler = false;
        var ranSecondary = false;
        var provider = new ServiceCollection()
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyTestClient>()            
            .AddRequestAndResponseCapturing(c => c
                .AddIsEnabledProvider<TestEnabledProvider>()
                .AddCapturingHandlerDelegate(context =>
                {
                    context.WithRequestAndResponse((req, res) => ranOtherHandler = true);
                    return Task.CompletedTask;
                })
                .AddCapturingHandlerDelegate(context =>
                {
                    return Task.CompletedTask;
                })
            )
            .AddRequestAndResponseCapturing(c => c
                .AddIsEnabledProvider()
                .AddIsEnabledProvider(c => c.IsEnabled = false)
                .AddCapturingHandlerDelegate(context =>
                {
                    ranSecondary = true;
                    return Task.CompletedTask;
                }))
            .Services
            .BuildServiceProvider();

        var client = provider.GetRequiredService<MyTestClient>();
        var mockHttpMessageHandler = provider.GetRequiredService<MockHttpMessageHandler>();

        mockHttpMessageHandler.When("http://nowhere.com").Respond(HttpStatusCode.OK);

        // Act
        await client.SendRequest();

        // Assert
        using var assertionScope = new AssertionScope();
        ranOtherHandler.Should().BeTrue();
        ranSecondary.Should().BeFalse();
    }

    [Test]
    public async Task GivenConfigureAllForHttpClients_ItShouldConfigureCapturingHandlersCorrectly()
    {
        // Arrange
        var ranOtherHandler = false;
        var ranSecondary = false;
        var provider = new ServiceCollection()
            .AddCapturingHandlersToAllHttpClients(c => c
                .AddCapturingHandlerDelegate(context => context.WithRequestAndResponse((req, res) => ranOtherHandler = true))
            )
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyOtherTestClient>()
            .Services
            .AddHttpClient<MyTestClient>()
            .AddRequestAndResponseCapturing(c => c
                .AddCapturingHandlerDelegate(context => ranSecondary = true)
            )
            .Services
            .BuildServiceProvider();

        var client = provider.GetRequiredService<MyTestClient>();
        var httpClient = provider.GetRequiredService<HttpClient>();
        var otherClient = provider.GetRequiredService<MyOtherTestClient>();        
        var mockHttpMessageHandler = provider.GetRequiredService<MockHttpMessageHandler>();

        mockHttpMessageHandler.When("http://nowhere.com").Respond(HttpStatusCode.OK);

        // Act
        await client.SendRequest();

        // Assert
        //using var assertionScope = new AssertionScope();
        ranOtherHandler.Should().BeFalse();
        ranSecondary.Should().BeTrue();

        ranSecondary = false;
        ranOtherHandler = false;

        await otherClient.SendRequest();

        ranOtherHandler.Should().BeTrue();
        ranSecondary.Should().BeFalse();

        ranSecondary = false;
        ranOtherHandler = false;

        await httpClient.GetAsync("http://nowhere.com");

        ;
    }
    
    [Test]
    public void GivenOptionsThatReceiveANullFactory_ItShouldThrowAnArgumentNullException()
    {
        Func<IServiceProvider, IAsyncRequestAndResponseCapturer> parameter = null!;
        new Action(() => new CapturingMessageHandlerOptions().AddCapturingHandlerFactory(parameter))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    private class TestEnabledProvider : IIsEnabledProvider
    {
        public bool IsEnabled => true;
    }
}
