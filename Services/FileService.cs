using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using ScheduleEditor.Models;
using ScheduleEditor.Services.Interfaces;

namespace ScheduleEditor.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ExportScheduleAsync(Schedule schedule)
        {
            var json = JsonConvert.SerializeObject(schedule, Formatting.Indented);
            var fileName = $"{schedule.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            await File.WriteAllTextAsync(filePath, json);
            return filePath;
        }

        public async Task<Schedule?> ImportScheduleAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<Schedule>(json);
        }

        public async Task ExportToExcelAsync(Schedule schedule, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Schedule");

            // Headers
            worksheet.Cells[1, 1].Value = "Step";
            worksheet.Cells[1, 2].Value = "Type";
            worksheet.Cells[1, 3].Value = "Mode";
            worksheet.Cells[1, 4].Value = "Charge Voltage";
            worksheet.Cells[1, 5].Value = "Discharge Voltage";
            worksheet.Cells[1, 6].Value = "Current";
            worksheet.Cells[1, 7].Value = "Power";
            worksheet.Cells[1, 8].Value = "End Conditions";
            worksheet.Cells[1, 9].Value = "Safety Conditions";

            // Data
            for (int i = 0; i < schedule.Steps.Count; i++)
            {
                var step = schedule.Steps[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = step.StepNumber;
                worksheet.Cells[row, 2].Value = step.Type.ToString();
                worksheet.Cells[row, 3].Value = step.Mode.ToString();
                worksheet.Cells[row, 4].Value = step.ChargeVoltage;
                worksheet.Cells[row, 5].Value = step.DischargeVoltage;
                worksheet.Cells[row, 6].Value = step.Current;
                worksheet.Cells[row, 7].Value = step.Power;
                worksheet.Cells[row, 8].Value = string.Join(", ", step.EndConditions);
                worksheet.Cells[row, 9].Value = string.Join(", ", step.StepSafeties);
            }

            worksheet.Cells.AutoFitColumns();
            await package.SaveAsAsync(new FileInfo(filePath));
        }

        public async Task<string> SelectFileAsync(string filter)
        {
            return await Task.Run(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = filter,
                    Multiselect = false
                };

                return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
            });
        }

        public async Task<string> SaveFileAsync(string filter, string defaultFileName)
        {
            return await Task.Run(() =>
            {
                var dialog = new SaveFileDialog
                {
                    Filter = filter,
                    FileName = defaultFileName
                };

                return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
            });
        }
    }
}