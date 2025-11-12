using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core;

internal class CapturingHandlerBuilder(string? name, IServiceCollection services) : ICapturingHandlerBuilder
{
    /// <inheritdoc/>
    public string Name => name!;

    /// <inheritdoc/>
    public IServiceCollection Services => services;
}