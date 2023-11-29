using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Parsers
{
    public class Time
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }

    public class ParserHTML : IParser
    {
        public static int LineWithGroupName = 10;

        // Массив с названиями месяцев в родительном падеже
        public static string[] monthNamesGenitive = new string[] {
            "января", "февраля", "марта", "апреля", "мая", "июня",
            "июля", "августа", "сентября", "октября", "ноября", "декабря"
        };

        public List<Lesson> Parse(Stream stream)
        {
            var lessons = new List<Lesson>();
            var times = new List<Time>();
            string groupName = "";
            var reader = new StreamReader(stream);

            int currentLine = 0;
            while (currentLine < 180)
            {
                var line = reader.ReadLine();
                currentLine++;

                if (currentLine == LineWithGroupName)
                    groupName = GetGroupName(line);
                else if (currentLine >= 36 && currentLine <= 46 && currentLine % 2 == 0)
                    times.Add(GetTimeStartAndEnd(line));
                else if (currentLine == 55 || currentLine == 76 || currentLine == 97 || currentLine == 118 || currentLine == 139 || currentLine == 160)
                {
                    DateTime date = GetDate(line);
                    MoveLessonsToParseResult(reader, ref currentLine, times, date, lessons, groupName);
                }
            }
            return lessons;
        }

        public static List<Lesson> MoveLessonsToParseResult(StreamReader reader, ref int currentLine, List<Time> times, DateTime date, List<Lesson> lessons, string GroupName)
        {
            for (int i = 0; i < 6; i++)
            {
                reader.ReadLine(); currentLine++;
                var line = reader.ReadLine(); currentLine++;

                string pattern = @"<P ALIGN=""CENTER"">(.*?)<BR>(.*?)<BR>";
                Match match = Regex.Match(line, pattern);

                string subjectNotReady = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(subjectNotReady))
                {
                    string subject = subjectNotReady;

                    string classroomTeacher = match.Groups[2].Value.Trim();

                    // Разбиваем classroomTeacher на слова
                    string[] words = classroomTeacher.Split(' ');
                    string classroom = words[words.Length - 1]; // Последнее слово
                    string teacher = classroomTeacher.Replace(classroom, "").Trim();

                    // Заполняем занятие
                    lessons.Add(new Lesson()
                    {
                        Group = new Domain.Group { Name = GroupName },
                        Teacher = new Teacher { Name = teacher },
                        Subject = new Subject { Name = subject },
                        Classroom = new Classroom { Name = classroom },
                        StartTime = times[i].Start,
                        EndTime = times[i].End,
                        Date = date
                    });
                }
            }

            return lessons;
        }

        public static DateTime GetDate(string? line)
        {
            DateTime resultDate = new DateTime();
            string pattern = @"<P ALIGN=""CENTER"">(.*?)</B>";

            Match match = Regex.Match(line, pattern);
            string dateStr = match.Groups[1].Value.Trim();
            string[] dateParts = dateStr.Split(',');
            string dayMonthPart = dateParts[1].Trim();

            string[] parts = dayMonthPart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int day;
            if (int.TryParse(parts[0], out day))
            {
                int month = Array.IndexOf(monthNamesGenitive, parts[1]);
                int year = DateTime.Now.Year; // Можно указать нужный год
                resultDate = new DateTime(year, month + 1, day);
            }

            return resultDate;
        }

        private static Time GetTimeStartAndEnd(string? line)
        {
            string pattern = "CENTER\">(.*?)</TD>";
            Match match = Regex.Match(line, pattern);
            string timeRange = match.Groups[1].Value.Trim();

            string[] parts = timeRange.Split('-');
            string startTimeStr = parts[0].Trim();
            string endTimeStr = parts[1].Trim();
            string timeFormat = "HH:mm";

            DateTime.TryParseExact(startTimeStr, timeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startTime);
            DateTime.TryParseExact(endTimeStr, timeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endTime);

            return new Time
            {
                Start = new TimeOnly(startTime.Hour, startTime.Minute),
                End = new TimeOnly(endTime.Hour, endTime.Minute)
            };
        }

        private static string GetGroupName(string? line)
        {
            string pattern = @"#ff00ff"">(.*?)<BR>";
            Match match = Regex.Match(line, pattern);
            string result = match.Groups[1].Value.Trim();
            return result;
        }
    }
}
