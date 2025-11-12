using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

internal class ExecutorBuilder(IServiceProvider serviceProvider) : IExecutorBuilder
{
    public Func<CaptureContext, Task> Build(IEnumerable<Func<IServiceProvider, IAsyncRequestAndResponseCapturer>> capturerFactories)
    {
        var factories = capturerFactories.Select(f => f(serviceProvider)).ToList();

        return async context =>
        {
            foreach (var handler in factories)
            {
                await handler.CaptureAsync(context).ConfigureAwait(false);
                if (context.ShouldStop) break;
            }
        };
    }
}