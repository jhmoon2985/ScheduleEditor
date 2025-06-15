using System.Threading.Tasks;
using System.Windows;
using ScheduleEditor.Services.Interfaces;

namespace ScheduleEditor.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowMessageAsync(string title, string message)
        {
            await Task.Run(() => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information));
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            return await Task.Run(() =>
                MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
        }

        public async Task<string?> ShowInputDialogAsync(string title, string prompt, string defaultValue = "")
        {
            // Simple input dialog implementation
            return await Task.Run(() =>
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultValue);
                return string.IsNullOrEmpty(input) ? null : input;
            });
        }

        public async Task ShowErrorAsync(string message)
        {
            await Task.Run(() => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        public async Task ShowInfoAsync(string message)
        {
            await Task.Run(() => MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information));
        }
    }
}