using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Models;

namespace ScheduleABMK.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> Register(UserDTO user);
        Task<AuthTokensDTO> Login(UserDTO user);
        Task<AuthTokensDTO> UpdateRefreshToken(string? oldRefreshToken);
    }
}
