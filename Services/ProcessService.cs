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
    public class ProcessService : IProcessService
    {
        private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "processes.json");
        private List<Process> _processes = new();

        public ProcessService()
        {
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IEnumerable<Process>> GetAllProcessesAsync()
        {
            await LoadDataAsync();
            return _processes;
        }

        public async Task<Process?> GetProcessByIdAsync(Guid id)
        {
            await LoadDataAsync();
            return _processes.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Process> CreateProcessAsync(string name, string description, string author)
        {
            var process = new Process
            {
                Name = name,
                Description = description,
                Author = author
            };
            _processes.Add(process);
            await SaveAsync();
            return process;
        }

        public async Task UpdateProcessAsync(Process process)
        {
            var index = _processes.FindIndex(p => p.Id == process.Id);
            if (index >= 0)
            {
                _processes[index] = process;
                await SaveAsync();
            }
        }

        public async Task DeleteProcessAsync(Guid id)
        {
            _processes.RemoveAll(p => p.Id == id);
            await SaveAsync();
        }

        public async Task<Process> CopyProcessAsync(Process source)
        {
            var copy = new Process
            {
                Name = $"{source.Name} - Copy",
                Description = source.Description,
                Author = source.Author
            };

            foreach (var schedule in source.Schedules)
            {
                var scheduleCopy = new Schedule
                {
                    Name = schedule.Name,
                    Description = schedule.Description,
                    Author = schedule.Author,
                    ScheduleSafety = schedule.ScheduleSafety,
                    TargetSafety = schedule.TargetSafety
                };

                foreach (var step in schedule.Steps)
                {
                    scheduleCopy.Steps.Add(step);
                }

                copy.Schedules.Add(scheduleCopy);
            }

            _processes.Add(copy);
            await SaveAsync();
            return copy;
        }

        public async Task SaveAsync()
        {
            var directory = Path.GetDirectoryName(_dataPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            var json = JsonConvert.SerializeObject(_processes, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }

        private async Task LoadDataAsync()
        {
            if (File.Exists(_dataPath))
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                _processes = JsonConvert.DeserializeObject<List<Process>>(json) ?? new List<Process>();
            }
        }
    }
}