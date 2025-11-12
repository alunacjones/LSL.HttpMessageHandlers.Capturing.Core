using System;
using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

/// <summary>
/// Capturing handler builder
/// </summary>
public interface ICapturingHandlerBuilder
{
    /// <summary>
    /// The name of the options
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The service collection 
    /// </summary>
    IServiceCollection Services { get; }        
}
