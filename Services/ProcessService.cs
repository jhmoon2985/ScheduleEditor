using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;

namespace ScheduleEditor.Services
{
    public class ProcessService : IProcessService
    {
        private readonly string _dataPath;
        private List<Process> _processes = new();
        private bool _isInitialized = false;

        public ProcessService()
        {
            // Data 폴더 경로 설정 및 생성
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            _dataPath = Path.Combine(dataDirectory, "processes.json");

            // 동기적 초기화로 변경하여 생성자에서 완료
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                LoadData();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                // 로그 또는 기본값으로 초기화
                _processes = new List<Process>();
                _isInitialized = true;

                // 기본 프로세스 생성
                CreateDefaultProcess();
            }
        }

        private void CreateDefaultProcess()
        {
            var defaultProcess = new Process
            {
                Name = "기본 공정",
                Description = "기본 공정입니다.",
                Author = "시스템"
            };
            _processes.Add(defaultProcess);

            // 기본 스케줄 추가
            var defaultSchedule = new Schedule
            {
                Name = "기본 스케줄",
                Description = "기본 스케줄입니다.",
                Author = "시스템"
            };
            defaultProcess.Schedules.Add(defaultSchedule);

            SaveAsync().Wait();
        }

        public async Task<IEnumerable<Process>> GetAllProcessesAsync()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
            return await Task.FromResult(_processes);
        }

        public async Task<Process?> GetProcessByIdAsync(Guid id)
        {
            if (!_isInitialized)
            {
                Initialize();
            }
            return await Task.FromResult(_processes.FirstOrDefault(p => p.Id == id));
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
                    ScheduleSafety = new ScheduleSafety
                    {
                        MaxVoltage = schedule.ScheduleSafety.MaxVoltage,
                        MaxCurrent = schedule.ScheduleSafety.MaxCurrent,
                        MaxPower = schedule.ScheduleSafety.MaxPower,
                        MaxTemperature = schedule.ScheduleSafety.MaxTemperature,
                        MinTemperature = schedule.ScheduleSafety.MinTemperature
                    },
                    TargetSafety = new TargetSafety
                    {
                        TargetVoltage = schedule.TargetSafety.TargetVoltage,
                        TargetCurrent = schedule.TargetSafety.TargetCurrent,
                        TargetPower = schedule.TargetSafety.TargetPower,
                        TargetTemperature = schedule.TargetSafety.TargetTemperature
                    }
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
            try
            {
                var directory = Path.GetDirectoryName(_dataPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                var json = JsonConvert.SerializeObject(_processes, Newtonsoft.Json.Formatting.Indented);
                await File.WriteAllTextAsync(_dataPath, json);
            }
            catch (Exception ex)
            {
                // 저장 실패 시 로그 처리
                System.Diagnostics.Debug.WriteLine($"ProcessService 저장 실패: {ex.Message}");
            }
        }

        private void LoadData()
        {
            if (File.Exists(_dataPath))
            {
                try
                {
                    var json = File.ReadAllText(_dataPath);
                    _processes = JsonConvert.DeserializeObject<List<Process>>(json) ?? new List<Process>();
                }
                catch (Exception ex)
                {
                    // 파일 로드 실패 시 빈 리스트로 초기화
                    _processes = new List<Process>();
                    System.Diagnostics.Debug.WriteLine($"ProcessService 로드 실패: {ex.Message}");
                }
            }
            else
            {
                _processes = new List<Process>();
            }
        }
    }
}