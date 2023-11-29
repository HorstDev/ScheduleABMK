using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Data.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IList<Subject>> GetAllAsync();
        Task AddAsync(Subject subject);
    }
}
