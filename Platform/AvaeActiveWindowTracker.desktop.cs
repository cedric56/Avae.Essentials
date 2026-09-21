#nullable enable

using Window = Avalonia.Controls.Window;

namespace Avae.Essentials;

class AvaeActiveWindowTracker
{
    readonly IAvaeWindowStateManager _windowStateManager;

    AvaeWindowMessageManager? _currentWindowManager;

    public AvaeActiveWindowTracker(IAvaeWindowStateManager windowStateManager)
    {
        _windowStateManager = windowStateManager;
    }

    public event EventHandler<WindowMessageEventArgs>? WindowMessage;

    public void Start()
    {
        var window = _windowStateManager.GetActiveWindow();
        OnActiveWindowChanged(window);

        _windowStateManager.ActiveWindowChanged += OnActiveWindowChanged;
    }

    public void Stop()
    {
        OnActiveWindowChanged(null);

        _windowStateManager.ActiveWindowChanged -= OnActiveWindowChanged;
    }

    void OnActiveWindowChanged(object? sender, EventArgs e)
    {
        var window = _windowStateManager?.GetActiveWindow();
        OnActiveWindowChanged(window);
    }

    void OnActiveWindowChanged(Window? window)
    {
        if (_currentWindowManager is not null)
        {
            _currentWindowManager.WindowMessage -= OnWindowMessage;
            _currentWindowManager = null;
        }

        if (window is not null)
        {
            _currentWindowManager = AvaeWindowMessageManager.Get(window);
            _currentWindowManager.WindowMessage += OnWindowMessage;
        }
    }

    void OnWindowMessage(object? sender, WindowMessageEventArgs e) =>
        WindowMessage?.Invoke(sender, e);
}
