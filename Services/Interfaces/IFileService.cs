using System.Threading.Tasks;
using ScheduleEditor.Models;

namespace ScheduleEditor.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> ExportScheduleAsync(Schedule schedule);
        Task<Schedule?> ImportScheduleAsync(string filePath);
        Task ExportToExcelAsync(Schedule schedule, string filePath);
        Task<string> SelectFileAsync(string filter);
        Task<string> SaveFileAsync(string filter, string defaultFileName);
    }
}