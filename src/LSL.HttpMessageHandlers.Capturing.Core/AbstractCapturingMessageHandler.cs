using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Abstract capturing message handler
/// </summary>
/// <param name="executorBuilder"></param>
public abstract class AbstractCapturingMessageHandler(IExecutorBuilder executorBuilder) : DelegatingHandler
{
    private readonly ConcurrentDictionary<string, Func<CaptureContext, Task>> _container = [];

    private Func<CaptureContext, Task> GetExecutor() => _container.GetOrAdd("value", _ => executorBuilder.Build(GetFactories()));

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

    /// <summary>
    /// Get the factories
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<Func<IAsyncRequestAndResponseCapturer>> GetFactories();
}
