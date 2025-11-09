using System;
using LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;
using LSL.HttpMessageHandlers.Capturing.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Message handlers core service collection extensions
/// </summary>
public static class MessageHandlersCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds request and response capturing to the client
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRequestAndResponseCapturing(this IHttpClientBuilder source, Action<CapturingMessageHandlerOptions>? configurator = null)
    {
        source.AssertNotNull(nameof(source)).Services
            .Configure(
                source.Name,
                configurator ?? (_ => { })
            )
            .AddTransient<CapturingMessageHandler>()
            .AddSingleton<IExecutorBuilder, ExecutorBuilder>();

        return source.AddHttpMessageHandler(
            sp => sp.GetRequiredService<CapturingMessageHandler>().With(a => a.Name = source.Name)
        );
    }

    /// <summary>
    /// Configure all named options for request and response capturing
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAllRequestAndResponseCapturing(this IServiceCollection source, Action<CapturingMessageHandlerOptions> configurator) =>
        source.AssertNotNull(nameof(source)).ConfigureAll(configurator);
}