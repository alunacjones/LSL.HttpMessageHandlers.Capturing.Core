using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// IExecutorBuilder
/// </summary>
public interface IExecutorBuilder
{
    /// <summary>
    /// Builds a request and response capturing delegate from all instances created by the provided factories
    /// </summary>
    /// <param name="capturerFactories"></param>
    /// <returns></returns>
    Func<CaptureContext, Task> Build(IEnumerable<Func<IAsyncRequestAndResponseCapturer>> capturerFactories);
}
