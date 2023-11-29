using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Application.Parsers;
using ScheduleABMK.Data;
using ScheduleABMK.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ScheduleABMK.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ScheduleDataContext _context;
        private IParser _parser;

        public LessonController(IHttpContextAccessor httpContextAccessor, ScheduleDataContext context, IParser parser)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _parser = parser;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            try
            {
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                if (httpRequest.Form.Files.Count < 1)
                {
                    return BadRequest("Файлы не загружены");
                }

                foreach (var file in httpRequest.Form.Files)
                {
                    var stream = file.OpenReadStream();
                    List<Lesson> lessons = _parser.Parse(stream);


                    foreach (var lesson in lessons)
                    {
                        Teacher teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Name == lesson.Teacher.Name);
                        // Проверка наличия учителя в базе данных по имени
                        if (teacher == null)
                        {
                            // Если учителя нет, добавляем его
                            teacher = new Teacher { Name = lesson.Teacher.Name };
                            await _context.Teachers.AddAsync(teacher);
                        }
                        lesson.Teacher = teacher; // Устанавливаем учителя для lesson

                        // Проверка наличия аудитории в базе данных по имени
                        Classroom classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Name == lesson.Classroom.Name);
                        if (classroom == null)
                        {
                            // Если аудитории нет, добавляем ее
                            classroom = new Classroom { Name = lesson.Classroom.Name };
                            await _context.Classrooms.AddAsync(classroom);
                        }
                        lesson.Classroom = classroom; // Устанавливаем аудиторию для lesson

                        // Проверка наличия группы в базе данных по имени
                        Group group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == lesson.Group.Name);
                        if (group == null)
                        {
                            // Если группы нет, добавляем ее
                            group = new Group { Name = lesson.Group.Name };
                            await _context.Groups.AddAsync(group);
                        }
                        lesson.Group = group; // Устанавливаем группу для lesson

                        // Проверка наличия предмета в базе данных по имени
                        Subject subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == lesson.Subject.Name);
                        if (subject == null)
                        {
                            // Если предмета нет, добавляем его
                            subject = new Subject { Name = lesson.Subject.Name };
                            await _context.Subjects.AddAsync(subject);
                        }
                        lesson.Subject = subject; // Устанавливаем предмет для lesson

                        // Сохраняем или обновляем lesson
                        if (lesson.Id == null || lesson.Id == Guid.Empty)
                        {
                            await _context.Lessons.AddAsync(lesson); // Добавляем новый урок
                        }
                        else
                        {
                            _context.Lessons.Update(lesson); // Обновляем существующий урок
                        }

                        await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
                    }
                }

                return Ok("Успешно");
            }
            catch (Exception e)
            {
                string error = e.Message;
                return StatusCode(500, e);
            }
        }
    }
}
