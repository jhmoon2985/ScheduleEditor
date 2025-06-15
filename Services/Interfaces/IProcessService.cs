using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleEditor.Models;

namespace ScheduleEditor.Services.Interfaces
{
    public interface IProcessService
    {
        Task<IEnumerable<Process>> GetAllProcessesAsync();
        Task<Process?> GetProcessByIdAsync(Guid id);
        Task<Process> CreateProcessAsync(string name, string description, string author);
        Task UpdateProcessAsync(Process process);
        Task DeleteProcessAsync(Guid id);
        Task<Process> CopyProcessAsync(Process source);
        Task SaveAsync();
    }
}
