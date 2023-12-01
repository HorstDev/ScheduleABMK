using ScheduleABMK.Application.Auth;

namespace ScheduleABMK.Server.Models
{
    public class AuthTokensDTO
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }

        public AuthTokensDTO(AccessToken accessToken, RefreshToken refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
