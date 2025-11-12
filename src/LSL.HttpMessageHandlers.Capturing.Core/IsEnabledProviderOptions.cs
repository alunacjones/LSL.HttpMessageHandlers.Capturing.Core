namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// A handler to enable or disable a chain of capturing handlers
/// </summary>
public class IsEnabledProviderOptions
{
    /// <summary>
    /// If <see langword="false"/> then further processing of handlers is disabled
    /// </summary>
    public bool IsEnabled { get; set; }
}
