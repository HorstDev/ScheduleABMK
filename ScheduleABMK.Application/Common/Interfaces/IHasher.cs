using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Common.Interfaces
{
    public interface IHasher
    {
        void CreateHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
