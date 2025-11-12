namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// The contract for an IIsEnabledProvider
/// </summary>
public interface IIsEnabledProvider
{
    /// <summary>
    /// Is enabled flag
    /// </summary>
    bool IsEnabled { get; }
}
