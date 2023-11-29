using ScheduleABMK.Application.Common.Interfaces;

namespace ScheduleABMK.Server.Services.Interfaces
{
    public interface ILessonService
    {
        Task<IResponse> UploadLessonsFromFilesAsync(IFormFileCollection files);
    }
}
