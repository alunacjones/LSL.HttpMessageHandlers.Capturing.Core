using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

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

