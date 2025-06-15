using System.Threading.Tasks;

namespace ScheduleEditor.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task<string?> ShowInputDialogAsync(string title, string prompt, string defaultValue = "");
        Task ShowErrorAsync(string message);
        Task ShowInfoAsync(string message);
    }
}