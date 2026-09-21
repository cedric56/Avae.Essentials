#nullable enable
using Avalonia.Controls.ApplicationLifetimes;
using Application = Avalonia.Application;
using Window = Avalonia.Controls.Window;

namespace Avae.Essentials;

/// <summary>
/// Manager object that manages window states on Windows.
/// </summary>
public interface IAvaeWindowStateManager
{
    /// <summary>
    /// Occurs when the application's active window changed.
    /// </summary>
    event EventHandler ActiveWindowChanged;

    /// <summary>
    /// Gets the application's currently active window.
    /// </summary>
    /// <returns>The application's currently active <see cref="Window"/> object.</returns>
    Window? GetActiveWindow();

    /// <summary>
    /// Occurs when a new window is created, but not yet displayed
    /// </summary>
    /// <param name="window">The <see cref="Window"/> object</param>
    void OnPlatformWindowInitialized(Window window);

    /// <summary>
    /// Sets the new active window that can be retrieved with <see cref="GetActiveWindow"/>.
    /// </summary>
    /// <param name="window">The <see cref="Window"/> object that is activated.</param>
    void OnActivated(Window window);
}

/// <summary>
/// Manager object that manages window states on Windows.
/// </summary>
public static class AvaeWindowStateManager
{
    static IAvaeWindowStateManager? defaultImplementation;

    /// <summary>
    /// Provides the default implementation for static usage of this API.
    /// </summary>
    public static IAvaeWindowStateManager Default =>
        defaultImplementation ??= new AvaeWindowStateManagerImplementation();

    internal static void SetDefault(IAvaeWindowStateManager? implementation) =>
        defaultImplementation = implementation;
}

static class AvaeWindowStateManagerExtensions
{
    /// <summary>
    /// Gets the application's currently active window.
    /// </summary>
    /// <param name="manager">The object to invoke this method on.</param>
    /// <param name="throwOnNull">Throws an exception if no current <see cref="Window"/> can be found and this value is set to <see langword="true"/>, otherwise this method returns <see langword="null"/>.</param>
    /// <returns>The application's currently active <see cref="Window"/> object.</returns>
    /// <exception cref="NullReferenceException">Thrown if no current <see cref="Window"/> can be found and <paramref name="throwOnNull"/> is set to <see langword="true"/>.</exception>
    public static Window? GetActiveWindow(this IAvaeWindowStateManager manager, bool throwOnNull)
    {
        var window = manager.GetActiveWindow();
        if (throwOnNull && window == null)
            throw new NullReferenceException("The active Window cannot be detected. Ensure that you have called Init in your Application class.");

        return window;
    }

    /// <summary>
    /// Gets the application's currently active window's pointer.
    /// </summary>
    /// <param name="manager">The object to invoke this method on.</param>
    /// <param name="throwOnNull">Throws an exception if no current <see cref="Window"/> can be found and this value is set to <see langword="true"/>, otherwise this method returns <see cref="IntPtr.Zero"/>.</param>
    /// <returns>The application's currently active window's <see cref="IntPtr"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown if no current <see cref="Window"/> can be found and <paramref name="throwOnNull"/> is set to <see langword="true"/>.</exception>
    public static IntPtr GetActiveWindowHandle(this IAvaeWindowStateManager manager, bool throwOnNull)
    {
        var window = manager.GetActiveWindow();
        if (throwOnNull && window == null)
            throw new NullReferenceException("The active Window cannot be detected. Ensure that you have called Init in your Application class.");

        if (window == null)
            return IntPtr.Zero;

        var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

        return handle;
    }
}

class AvaeWindowStateManagerImplementation : IAvaeWindowStateManager
{
    Window? _activeWindow;

    public event EventHandler? ActiveWindowChanged;

    public Window? GetActiveWindow() =>
        _activeWindow ??
        (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ?
        desktop.MainWindow :
        null);

    void SetActiveWindow(Window window)
    {
        if (_activeWindow == window)
            return;

        _activeWindow = window;

        ActiveWindowChanged?.Invoke(window, EventArgs.Empty);
    }

    public void OnPlatformWindowInitialized(Window window)
    {
        SetActiveWindow(window);
    }

    public void OnActivated(Window window)
    {
        SetActiveWindow(window);
    }
}
