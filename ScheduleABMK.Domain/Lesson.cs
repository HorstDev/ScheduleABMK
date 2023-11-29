using System.Text.RegularExpressions;

namespace ScheduleABMK.Domain
{
    public class Lesson
    {
        public Guid? Id { get; set; }

        public Group? Group { get; set; }
        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
        public Classroom? Classroom { get; set; }

        public Guid? GroupId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? ClassroomId { get; set; }
        public Guid? SubjectId { get; set; }

        public DateTime? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
