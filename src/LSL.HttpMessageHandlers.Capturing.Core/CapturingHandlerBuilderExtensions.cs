using System;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Capturing handler builder extensions
/// </summary>
public static class CapturingHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a capturing handler factory method
    /// </summary>
    /// <param name="source"></param>
    /// <param name="factory"></param>
    /// <param name="index"></param>
    /// <returns></returns>    
    public static ICapturingHandlerBuilder AddCapturingHandlerFactory(
        this ICapturingHandlerBuilder source,
        Func<IServiceProvider, IAsyncRequestAndResponseCapturer> factory,
        int? index = null
    ) =>
    source.Configure<CapturingMessageHandlerOptions>(source.Name, c => c.AddCapturingHandlerFactory(factory, index));

    /// <summary>
    /// Adds a capturing delegate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="delegate"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ICapturingHandlerBuilder AddCapturingHandlerDelegate(
        this ICapturingHandlerBuilder source,
        Func<CaptureContext, Task> @delegate,
        int? index = null
    ) =>
    source.Configure<CapturingMessageHandlerOptions>(source.Name, c => c.AddCapturingHandlerFactory(_ =>
        new DelegatingAsyncRequestAndResponseCapturer(@delegate),
        index
    ));

    /// <summary>
    /// Adds a capturing delegate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="delegate"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ICapturingHandlerBuilder AddCapturingHandlerDelegate(
        this ICapturingHandlerBuilder source,
        Action<CaptureContext> @delegate,
        int? index = null
    ) =>
    source.AddCapturingHandlerDelegate(
        context =>
        {
            @delegate(context);
            return Task.CompletedTask;
        },
        index
    );

    /// <summary>
    /// Builds a unique name based on the 
    /// <see cref="ICapturingHandlerBuilder">ICapturingHandlerBuilder</see>'s 
    /// <see cref="ICapturingHandlerBuilder.Name">Name</see>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string BuildUniqueName(this ICapturingHandlerBuilder source) => OptionsHelper.BuildUniqueName(source.Name);
}
