using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleEditor.Models;

namespace ScheduleEditor.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GetSchedulesByProcessIdAsync(Guid processId);
        Task<Schedule?> GetScheduleByIdAsync(Guid id);
        Task<Schedule> CreateScheduleAsync(Guid processId, string name, string description, string author);
        Task UpdateScheduleAsync(Schedule schedule);
        Task DeleteScheduleAsync(Guid id);
        Task<Schedule> CopyScheduleAsync(Schedule source);
        Task SaveAsync();
    }
}