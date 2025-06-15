using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ScheduleEditor.Models
{
    public class Process
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObservableCollection<Schedule> Schedules { get; set; } = new();
    }
}