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
            .AddCapturingHandlersToAllHttpClients(c => c
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
        var ranOtherHandler = 0;
        var ranSecondary = 0;
        var provider = new ServiceCollection()
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyTestClient>()
            .AddRequestAndResponseCapturing(c => c
                .AddIsEnabledProvider<TestEnabledProvider>()
                .AddCapturingHandlerDelegate(context =>
                {
                    context.WithRequestAndResponse((req, res) => ranOtherHandler++);
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
                    ranSecondary++;
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
        ranOtherHandler.Should().Be(1);
        ranSecondary.Should().Be(0);
    }

    [TestCase(false, new bool[] { false, true })]
    [TestCase(true, new bool[] { false, true })]
    public async Task GivenConfigureAllForHttpClients_ItShouldConfigureCapturingHandlersCorrectly(bool registerGlobalFirst, bool[] expectedRunOrder)
    {
        // Arrange
        var ranGlobalHandler = 0;
        var ranSecondary = 0;
        var runOrder = new List<bool>();

        var provider = new ServiceCollection()
            .ExecuteIf(registerGlobalFirst, o => o.AddCapturingHandlersToAllHttpClients(ConfigureGlobals))
            .AddMockHttpMessageHandler()
            .AddHttpClient<MyOtherTestClient>()
            .Services
            .AddHttpClient<MyTestClient>()
            .AddRequestAndResponseCapturing(c => c
                .AddCapturingHandlerDelegate(context => { ranSecondary++; runOrder.Add(false); })
            )
            .Services
            .ExecuteIf(registerGlobalFirst is false, o => o.AddCapturingHandlersToAllHttpClients(ConfigureGlobals))
            .BuildServiceProvider();

        void ConfigureGlobals(ICapturingHandlerBuilder builder) => builder
            .AddCapturingHandlerDelegate(context => context.WithRequestAndResponse((req, res) => { ranGlobalHandler++; runOrder.Add(true); }))
            .AddCapturingHandler<TestHandler>();

        var client = provider.GetRequiredService<MyTestClient>();
        var httpClient = provider.GetRequiredService<HttpClient>();
        var otherClient = provider.GetRequiredService<MyOtherTestClient>();
        var mockHttpMessageHandler = provider.GetRequiredService<MockHttpMessageHandler>();

        mockHttpMessageHandler.When("http://nowhere.com").Respond(HttpStatusCode.OK);

        // Act
        await client.SendRequest();

        // Assert
        //using var assertionScope = new AssertionScope();

        ranGlobalHandler.Should().Be(1);
        ranSecondary.Should().Be(1);
        runOrder.Should().BeEquivalentTo(expectedRunOrder, o => o.WithStrictOrdering());

        ClearRecordedData();

        await otherClient.SendRequest();

        //ranOtherHandler.Should().BeTrue();
        ranSecondary.Should().Be(0);

        ClearRecordedData();

        await httpClient.GetAsync("http://nowhere.com");

        ranGlobalHandler.Should().Be(1);
        ranSecondary.Should().Be(0);

        runOrder.Should().BeEquivalentTo([true], o => o.WithStrictOrdering());

        void ClearRecordedData()
        {
            ranSecondary = 0;
            ranGlobalHandler = 0;
            runOrder.Clear();
        }
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

    private class TestHandler : IAsyncRequestAndResponseCapturer
    {
        public Task CaptureAsync(CaptureContext context)
        {
            return Task.CompletedTask;
        }
    }
}
