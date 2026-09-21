using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Maui.Essentials;
using Application = Avalonia.Application;

namespace Avae.Essentials;

public class AvaeTopLevelStateManager : IAvaloniaEssentialsPlatformProvider
{
    TopLevel? _active;

    public AvaeTopLevelStateManager()
    {
        TopLevel.GotFocusEvent.AddClassHandler(typeof(TopLevel), (sender, args) =>
        {
            OnActivated((TopLevel)sender!);
        });
    }

    public void OnActivated(TopLevel topLevel)
    {
        if (_active == topLevel)
            return;

        _active = topLevel;
    }

    public TopLevel? GetTopLevel()
    {
        var lifetime = Application.Current?.ApplicationLifetime;
        var active = lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => _active ?? TopLevel.GetTopLevel(desktop.MainWindow),
            ISingleViewApplicationLifetime singleView => _active ?? TopLevel.GetTopLevel(singleView.MainView),
            _ => _active ?? TopLevel.GetTopLevel(null)
        };

        return active ?? TopLevel.GetTopLevel(null);
    }
}
