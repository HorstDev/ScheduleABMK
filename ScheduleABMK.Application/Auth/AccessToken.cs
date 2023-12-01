using Microsoft.IdentityModel.Tokens;
using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Application.Auth
{
    public class AccessToken : IToken
    {
        public string Token { get; set; }

        public AccessToken(User user, SymmetricSecurityKey secretKey, int lifeTimeInMinutes)
        {
            Token = Create(user, secretKey, lifeTimeInMinutes);
        }

        private string Create(User user, SymmetricSecurityKey secretKey, int lifeTimeInMinutes)
        {
            // в claims будет лежать вся информация о пользователе, которая нам нужна в токене
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(lifeTimeInMinutes),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
