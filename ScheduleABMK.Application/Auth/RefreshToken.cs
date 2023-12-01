using ScheduleABMK.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Auth
{
    public class RefreshToken : IToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }

        public RefreshToken(int lifeTimeInDays)
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            Expires = DateTime.Now.AddDays(lifeTimeInDays);
            Created = DateTime.Now;
        }
    }
}
