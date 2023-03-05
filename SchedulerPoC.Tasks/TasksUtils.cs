using SchedulerPoC.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SchedulerPoC.Tasks
{
    public static class TasksUtils
    {

        public static IEither<string, ScheduledTasksList> LoadTasksFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return new Left<string, ScheduledTasksList>($"File not found: {path}");
            }

            var errors = new List<string>();
            var result = new List<ScheduledTaskStatus>();
            foreach (var line in File.ReadAllLines(path))
            {
                if (line.Length > 0)
                {
                    if (line.StartsWith("#"))
                    {
                        // comment
                    }
                    else
                    {
                        line.AsTask()
                            .Match(
                                error => errors.Add(error),
                                task => result.Add(task.WithTimeStatus()));
                    }
                }
            }
            
            if (errors.Any())
            {
                var asStr = String.Join("\n\t", errors.ToArray());
                return new Left<string, ScheduledTasksList>($"Errors found when loading schedules:\n{asStr}");
            }
            else
            {
                return new Right<string, ScheduledTasksList>(new ScheduledTasksList(result));
            }
        }

        public static IEither<string, ScheduledTask> AsTask(this string src)
        {
            var reXls = new Regex("(\\s?\\d+):(\\d+) XL\\(([^,)]+), ([^)]+)\\)");
            var rePy = new Regex("(\\s?\\d+):(\\d+) PY\\(([^,)]+), ([^,)]+), \\[([^\\]]*)\\], ([^)]+)\\)");

            var m = reXls.Match(src);
            if (m.Success)
            {
                if (!Int32.TryParse(m.Groups[1].Value, out var hour))
                {
                    return new Left<string, ScheduledTask>($"Invalid hour: {src}");
                }

                if (!Int32.TryParse(m.Groups[2].Value, out var minutes))
                {
                    return new Left<string, ScheduledTask>($"Invalid minutes: {src}");
                }

                var task = new ExcelTask(m.Groups[3].Value, m.Groups[4].Value);
                var time = DateTime.Today.AddHours(hour).AddMinutes(minutes);

                return new Right<string, ScheduledTask>(new ScheduledTask(GetNewKey(), task, time));
            }

            m = rePy.Match(src);
            if (m.Success)
            {
                if (!Int32.TryParse(m.Groups[1].Value, out var hour))
                {
                    return new Left<string, ScheduledTask>($"Invalid hour: {src}");
                }

                if (!Int32.TryParse(m.Groups[2].Value, out var minutes))
                {
                    return new Left<string, ScheduledTask>($"Invalid minutes: {src}");
                }

                string paramsAsStr =  m.Groups[5].Value ?? "";
                string[] parameters = paramsAsStr.Split(',').Select(r => r?.Trim()).Where(r => !string.IsNullOrEmpty(r)).ToArray();

                var task = new PythonTask(m.Groups[3].Value, m.Groups[4].Value, parameters, m.Groups[6].Value);
                var time = DateTime.Today.AddHours(hour).AddMinutes(minutes);

                return new Right<string, ScheduledTask>(new ScheduledTask(GetNewKey(), task, time));
            }

            return new Left<string, ScheduledTask>($"Unknown: {src}");
        }

        public static ScheduledTaskStatus WithTimeStatus(this ScheduledTask task)
        {
            if (task.Schedule.Ticks >= DateTime.Now.Ticks)
            {
                return new ScheduledTaskStatus(task, ScheduleStatus.Past);
            }

            return new ScheduledTaskStatus(task, ScheduleStatus.Waiting);
        }

        public static string GetNewKey()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
