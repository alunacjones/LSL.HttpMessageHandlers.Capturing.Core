using System;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;
using LSL.HttpMessageHandlers.Capturing.Core;
using Microsoft.Extensions.Http;
using System.Net.Http;
using Microsoft.Extensions.Options;

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
    /// Adds a capturing message handler to all <see cref="HttpClient"/>s
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IServiceCollection AddCapturingHandlersToAllHttpClients(this IServiceCollection source, Action<ICapturingHandlerBuilder> configurator)
    {
        var builder = new CapturingHandlerBuilder(CapturingMessageHandlerOptions.GlobalSettingsName, source);
        configurator.AssertNotNull(nameof(configurator))(builder);

        return source
            .AddCapturingHandlerServices()
            .AddOptions<CapturingMessageHandlerOptions>(builder.Name)
            .Services
            .ConfigureAll<HttpClientFactoryOptions>(o =>
            {
                o.HttpMessageHandlerBuilderActions.Insert(
                    0,
                    m => m.AdditionalHandlers.Insert(0, m.Services.GetRequiredService<CapturingMessageHandler>().With(h => h.Name = builder.Name)
                ));
            });
    }

    internal static IServiceCollection AddCapturingHandlerServices(this IServiceCollection source) =>
        source
            .FluentlyTryAddSingleton<IExecutorBuilder, ExecutorBuilder>()
            .FluentlyTryAddTransient<CapturingMessageHandler>();        
}       