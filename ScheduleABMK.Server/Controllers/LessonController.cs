using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Application.Parsers;
using ScheduleABMK.Data;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Services.Interfaces;

namespace ScheduleABMK.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpPost("upload-files"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> UploadFiles(IFormFileCollection files)
        {
            await _lessonService.UploadLessonsFromFilesAsync(files);

            return Ok("Файлы успешно загружены!");
        }
    }
}
