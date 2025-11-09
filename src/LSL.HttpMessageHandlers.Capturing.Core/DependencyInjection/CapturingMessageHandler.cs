using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

internal class CapturingMessageHandler(
    IOptionsSnapshot<CapturingMessageHandlerOptions> optionsSnapshot,
    IExecutorBuilder executorBuilder,
    IServiceProvider serviceProvider) 
    : AbstractCapturingMessageHandler(executorBuilder)
{
    internal string Name { get; set; } = string.Empty;

    protected override IEnumerable<Func<IAsyncRequestAndResponseCapturer>> GetFactories()
    {
        foreach (var factory in optionsSnapshot.Get(Name).Factories)
        {
            yield return () => factory(serviceProvider);
        }
    }
}
