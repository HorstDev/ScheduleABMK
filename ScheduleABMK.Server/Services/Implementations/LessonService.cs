using Microsoft.EntityFrameworkCore;
using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Data;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Services.Interfaces;

namespace ScheduleABMK.Server.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private ScheduleDataContext _context;
        private IParser _parser;

        public LessonService(ScheduleDataContext context, IParser parser)
        {
            _context = context;
            _parser = parser;
        }

        public async Task UploadLessonsFromFilesAsync(IFormFileCollection files)
        {
            try
            {
                if (files.Count < 1)
                {
                    throw new BadHttpRequestException("Файлы не были загружены");
                }

                foreach (var file in files)
                {
                    await ProcessFileAsync(file);
                }
            }
            catch (Exception ex)
            {
                // тут можно реализовать логгирование
                throw; // перебрасываем исключение для того, чтобы его поймал middleware
            }
        }

        public async Task ProcessFileAsync(IFormFile file)
        {
            var stream = file.OpenReadStream();
            List<Lesson> lessons = _parser.Parse(stream);
            await ProcessAndSaveLessonsAsync(lessons);
        }

        private async Task ProcessAndSaveLessonsAsync(List<Lesson> lessons)
        {
            var teachers = await _context.Teachers.Distinct().ToListAsync();
            var classrooms = await _context.Classrooms.Distinct().ToListAsync();
            var groups = await _context.Groups.Distinct().ToListAsync();
            var subjects = await _context.Subjects.Distinct().ToListAsync();

            foreach (Lesson lesson in lessons)
            {
                Teacher? teacher = teachers.FirstOrDefault(x => x.Name == lesson.Teacher.Name);
                // Проверка наличия учителя в базе данных по имени
                if (teacher == null)
                {
                    // Если учителя нет, добавляем его
                    teacher = new Teacher { Name = lesson.Teacher.Name };
                    teachers.Add(teacher); // добавляем в локальные имена учителей
                    await _context.Teachers.AddAsync(teacher);
                }
                lesson.Teacher = teacher; // Устанавливаем учителя для lesson

                // Проверка наличия аудитории в базе данных по имени
                Classroom? classroom = classrooms.FirstOrDefault(c => c.Name == lesson.Classroom.Name);
                if (classroom == null)
                {
                    // Если аудитории нет, добавляем ее
                    classroom = new Classroom { Name = lesson.Classroom.Name };
                    classrooms.Add(classroom);
                    await _context.Classrooms.AddAsync(classroom);
                }
                lesson.Classroom = classroom; // Устанавливаем аудиторию для lesson

                // Проверка наличия группы в базе данных по имени
                Group? group = groups.FirstOrDefault(g => g.Name == lesson.Group.Name);
                if (group == null)
                {
                    // Если группы нет, добавляем ее
                    group = new Group { Name = lesson.Group.Name };
                    groups.Add(group);
                    await _context.Groups.AddAsync(group);
                }
                lesson.Group = group; // Устанавливаем группу для lesson

                // Проверка наличия предмета в базе данных по имени
                Subject? subject = subjects.FirstOrDefault(s => s.Name == lesson.Subject.Name);
                if (subject == null)
                {
                    // Если предмета нет, добавляем его
                    subject = new Subject { Name = lesson.Subject.Name };
                    subjects.Add(subject);
                    await _context.Subjects.AddAsync(subject);
                }
                lesson.Subject = subject; // Устанавливаем предмет для lesson

                await _context.Lessons.AddAsync(lesson); // Добавляем новый урок
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
        }
    }
}
