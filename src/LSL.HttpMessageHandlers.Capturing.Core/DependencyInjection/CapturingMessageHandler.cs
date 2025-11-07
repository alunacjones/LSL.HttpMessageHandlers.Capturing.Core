using System;
using System.Collections.Generic;
using LSL.HttpMessageHandlers.Capturing.Core;
using LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
