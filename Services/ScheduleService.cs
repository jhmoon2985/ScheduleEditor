using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;

namespace ScheduleEditor.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "schedules.json");
        private List<Schedule> _schedules = new();

        public ScheduleService()
        {
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByProcessIdAsync(Guid processId)
        {
            await LoadDataAsync();
            return _schedules.Where(s => s.Id == processId);
        }

        public async Task<Schedule?> GetScheduleByIdAsync(Guid id)
        {
            await LoadDataAsync();
            return _schedules.FirstOrDefault(s => s.Id == id);
        }

        public async Task<Schedule> CreateScheduleAsync(Guid processId, string name, string description, string author)
        {
            var schedule = new Schedule
            {
                Name = name,
                Description = description,
                Author = author
            };
            _schedules.Add(schedule);
            await SaveAsync();
            return schedule;
        }

        public async Task UpdateScheduleAsync(Schedule schedule)
        {
            var index = _schedules.FindIndex(s => s.Id == schedule.Id);
            if (index >= 0)
            {
                _schedules[index] = schedule;
                await SaveAsync();
            }
        }

        public async Task DeleteScheduleAsync(Guid id)
        {
            _schedules.RemoveAll(s => s.Id == id);
            await SaveAsync();
        }

        public async Task<Schedule> CopyScheduleAsync(Schedule source)
        {
            var copy = new Schedule
            {
                Name = $"{source.Name} - Copy",
                Description = source.Description,
                Author = source.Author,
                ScheduleSafety = source.ScheduleSafety,
                TargetSafety = source.TargetSafety
            };

            foreach (var step in source.Steps)
            {
                copy.Steps.Add(step);
            }

            _schedules.Add(copy);
            await SaveAsync();
            return copy;
        }

        public async Task SaveAsync()
        {
            var directory = Path.GetDirectoryName(_dataPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            var json = JsonConvert.SerializeObject(_schedules, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }

        private async Task LoadDataAsync()
        {
            if (File.Exists(_dataPath))
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                _schedules = JsonConvert.DeserializeObject<List<Schedule>>(json) ?? new List<Schedule>();
            }
        }
    }
}