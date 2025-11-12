using System;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Helper function container for options
/// </summary>
public static class OptionsHelper
{
    /// <summary>
    /// Builds a unique name using the <paramref name="originalName"/>
    /// </summary>
    /// <param name="originalName">The original name to generate a new unique name from</param>
    /// <param name="preserveNull">
    /// If <see langword="true"/> and <paramref name="originalName"/> is null then null is returned.
    /// Otherwise, a unique name is generated.
    /// </param>
    /// <returns></returns>
    public static string BuildUniqueName(string? originalName, bool preserveNull = false) => originalName is null 
        ? preserveNull ? null! : $"{Guid.NewGuid()}"
        : $"{originalName}-{Guid.NewGuid()}";
}