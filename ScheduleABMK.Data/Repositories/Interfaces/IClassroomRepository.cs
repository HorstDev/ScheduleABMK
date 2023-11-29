using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Data.Repositories.Interfaces
{
    public interface IClassroomRepository
    {
        Task<IList<Classroom>> GetAllAsync();
        Task AddAsync(Classroom classroom);
    }
}
