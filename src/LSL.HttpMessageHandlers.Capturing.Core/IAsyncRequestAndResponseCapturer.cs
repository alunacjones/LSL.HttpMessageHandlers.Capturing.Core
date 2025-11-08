using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// IAsyncRequestAndResponseCapturer
/// </summary>
public interface IAsyncRequestAndResponseCapturer
{
    /// <summary>
    /// Captures a request and response context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task CaptureAsync(CaptureContext context);
}

