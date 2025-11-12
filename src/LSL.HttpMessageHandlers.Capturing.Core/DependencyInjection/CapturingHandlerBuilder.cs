using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

internal class CapturingHandlerBuilder(string? name, IServiceCollection services) : ICapturingHandlerBuilder
{
    private readonly string _name = InternalCapturingHandlerExtensions.BuildUniqueName(name);

    /// <inheritdoc/>
    public string Name => _name;

    /// <inheritdoc/>
    public IServiceCollection Services => services;
}