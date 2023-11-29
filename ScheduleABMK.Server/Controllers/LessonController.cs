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
        private IHttpContextAccessor _httpContextAccessor;
        private ILessonService _lessonService;

        public LessonController(IHttpContextAccessor httpContextAccessor, ILessonService lessonService)
        {
            _httpContextAccessor = httpContextAccessor;
            _lessonService = lessonService;
        }

        [HttpPost("upload-files")]
        public async Task<ActionResult> UploadFiles()
        {
            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;

                if (files.Count < 1)
                    return BadRequest("Файлы не загружены");

                var response = await _lessonService.UploadLessonsFromFilesAsync(files);

                return Ok(response.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
