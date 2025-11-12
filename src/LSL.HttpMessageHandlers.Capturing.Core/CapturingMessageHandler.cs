using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace LSL.HttpMessageHandlers.Capturing.Core;

internal class CapturingMessageHandler(
    IOptionsSnapshot<CapturingMessageHandlerOptions> optionsSnapshot,
    IExecutorBuilder executorBuilder) : DelegatingHandler
{
    private readonly ConcurrentDictionary<string, Func<CaptureContext, Task>> _container = [];

    private Func<CaptureContext, Task> GetExecutor() => _container.GetOrAdd(
        "value",
        _ => executorBuilder.Build(optionsSnapshot.Get(Name).HandlerFactories));
        
    internal string? Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            await GetExecutor()(new CaptureContext(request, response, null));
            return response;
        }
        catch (Exception ex)
        {
            await GetExecutor()(new CaptureContext(request, null, ex));
            throw;
        }
    }    
}
