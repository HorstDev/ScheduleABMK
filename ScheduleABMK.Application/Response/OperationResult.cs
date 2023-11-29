using ScheduleABMK.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Response
{
    public class OperationResult : IResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
