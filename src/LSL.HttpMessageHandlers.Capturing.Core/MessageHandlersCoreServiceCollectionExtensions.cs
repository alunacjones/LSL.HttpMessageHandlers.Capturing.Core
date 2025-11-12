using System;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;
using LSL.HttpMessageHandlers.Capturing.Core;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

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
    public static IHttpClientBuilder AddRequestAndResponseCapturing(this IHttpClientBuilder source, Action<ICapturingHandlerBuilder>? configurator = null)
    {
        var builder = new CapturingHandlerBuilder(OptionsHelper.BuildUniqueName(source.Name), source.Services);

        source.AssertNotNull(nameof(source)).Services
            .AddCapturingHandlerServices();

        configurator?.Invoke(builder);

        return source.AddHttpMessageHandler(
            sp => sp.GetRequiredService<CapturingMessageHandler>().With(
                a => a.Name = builder.Name
            )
        );
    }

    /// <summary>
    /// Configure all named options for request and response capturing
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAllRequestAndResponseCapturing(this IServiceCollection source, Action<ICapturingHandlerBuilder> configurator)
    {
        var builder = new CapturingHandlerBuilder(null, source.AssertNotNull(nameof(source)));
        configurator.AssertNotNull(nameof(configurator)).Invoke(builder);
        return source;
    }

    /// <summary>
    /// Adds a capturing message handler to all <see cref="HttpClient"/>s
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IServiceCollection AddCapturingHandlersToAllHttpClients(this IServiceCollection source)
    {
        return source
            .AddCapturingHandlerServices()
            .ConfigureAll<HttpClientFactoryOptions>(o =>
            {
                o.HttpMessageHandlerBuilderActions.Add(
                    m => m.Services.GetRequiredService<CapturingMessageHandler>().With(h => h.Name = m.Name)
                );
            });
    }

    internal static IServiceCollection AddCapturingHandlerServices(this IServiceCollection source) =>
        source
            .FluentlyTryAddSingleton<IExecutorBuilder, ExecutorBuilder>()
            .FluentlyTryAddTransient<CapturingMessageHandler>();        
}       