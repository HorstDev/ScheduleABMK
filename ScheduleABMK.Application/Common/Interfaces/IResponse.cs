using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Common.Interfaces
{
    public interface IResponse
    {
        bool IsSuccess { get; set; }
        string? Message { get; set; }
    }
}
