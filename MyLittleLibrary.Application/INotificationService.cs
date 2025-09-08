using System;
using System.Threading.Tasks;

namespace MyLittleLibrary.Application;

public interface INotificationService
{
    void Success(string message, string? key = null, int? durationMs = null);
    void Info(string message, string? key = null, int? durationMs = null);
    void Warning(string message, string? key = null, int? durationMs = null);
    void Error(string message, string? key = null, int? durationMs = null);
    void Error(Exception ex, string? messagePrefix = null, string? key = null, int? durationMs = null);
}
