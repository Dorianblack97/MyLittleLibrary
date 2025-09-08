using System;
using MyLittleLibrary.Application;
using MudBlazor;

namespace MyLittleLibrary.Services;

public class NotificationService : INotificationService
{
    private readonly ISnackbar _snackbar;

    public NotificationService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
        // Default configuration (optional)
        _snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        _snackbar.Configuration.VisibleStateDuration = 4000;
        _snackbar.Configuration.HideTransitionDuration = 200;
        _snackbar.Configuration.ShowTransitionDuration = 200;
        _snackbar.Configuration.SnackbarVariant = Variant.Filled;
    }

    public void Success(string message, string? key = null, int? durationMs = null)
        => Add(message, Severity.Success, key, durationMs);

    public void Info(string message, string? key = null, int? durationMs = null)
        => Add(message, Severity.Info, key, durationMs);

    public void Warning(string message, string? key = null, int? durationMs = null)
        => Add(message, Severity.Warning, key, durationMs);

    public void Error(string message, string? key = null, int? durationMs = null)
        => Add(message, Severity.Error, key, durationMs);

    public void Error(Exception ex, string? messagePrefix = null, string? key = null, int? durationMs = null)
    {
        var msg = string.IsNullOrWhiteSpace(messagePrefix) ? ex.Message : $"{messagePrefix}: {ex.Message}";
        Add(msg, Severity.Error, key, durationMs);
    }

    private void Add(string message, Severity severity, string? key, int? durationMs)
    {
        var opt = durationMs.HasValue ? new Action<SnackbarOptions>(o => o.VisibleStateDuration = durationMs.Value) : null;
        if (opt is null)
            _snackbar.Add(message: message, severity: severity, key: key);
        else
            _snackbar.Add(message, severity, opt, key);
    }
}
