using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Is enabled provider
/// </summary>
/// <param name="isEnabledProvider"></param>
public class IsEnabledCapturingHandler(IIsEnabledProvider isEnabledProvider) : IAsyncRequestAndResponseCapturer
{
    /// <inheritdoc/>
    public Task CaptureAsync(CaptureContext context)
    {
        if (isEnabledProvider.IsEnabled is false)
        {
            context.StopProcessing();
        }

        return Task.CompletedTask;
    }
}

internal class IsEnabledOptionsContainer(string name, IOptionsMonitor<IsEnabledProviderOptions> optionsMonitor) : IIsEnabledProvider
{
    public bool IsEnabled => optionsMonitor.Get(name).IsEnabled;
}