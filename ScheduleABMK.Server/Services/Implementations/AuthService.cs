using Microsoft.IdentityModel.Tokens;
using ScheduleABMK.Application.Auth;
using ScheduleABMK.Application.Common.Interfaces;
using ScheduleABMK.Data.Repositories.Interfaces;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Models;
using ScheduleABMK.Server.Services.Interfaces;

namespace ScheduleABMK.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private PasswordHasher _passwordHasher;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, IRoleRepository roleRepository, PasswordHasher passwordHasher)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> Register(UserDTO request)
        {
            try
            {
                User? user = await _userRepository.GetByEmail(request.Email);
                if (user == null)
                {
                    _passwordHasher.CreateHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = request.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    Role userRole = (await _roleRepository.GetByName("Student"))!;
                    if (userRole != null)
                        user.Role = userRole;
                    await _userRepository.Create(user);

                    return user;
                }
                else
                {
                    throw new BadHttpRequestException("Пользователь с такими данными уже существует!");
                }
            }
            catch(Exception ex)
            {
                // тут можно реализовать логгирование
                throw; // перебрасываем исключение для того, чтобы его поймал middleware
            }
        }

        public async Task<AuthTokensDTO> Login(UserDTO request)
        {
            try
            {
                User? user = await _userRepository.GetByEmail(request.Email);
                if (user == null)
                {
                    throw new BadHttpRequestException("Пользователь не найден!");
                }

                if (!_passwordHasher.VerifyHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    throw new BadHttpRequestException("Неверный пароль!");
                }

                var accessToken = CreateJWT(user);
                var refreshToken = new RefreshToken(30);

                // При каждой авторизации пользователя обновляем ему refresh token в бд
                user.RefreshToken = refreshToken.Token;
                user.TokenCreated = refreshToken.Created;
                user.TokenExpires = refreshToken.Expires;
                await _userRepository.Update(user);

                return new AuthTokensDTO(accessToken, refreshToken);
            }
            catch(Exception ex)
            {
                // тут можно реализовать логгирование
                throw; // перебрасываем исключение для того, чтобы его поймал middleware
            }
        }

        public async Task<AuthTokensDTO> UpdateRefreshToken(string? oldRefreshToken)
        {
            try
            {
                //var refreshToken = Request.Cookies["refreshToken"];
                var user = await _userRepository.GetByRefreshToken(oldRefreshToken); // P.S. наверное надо выставить индексы на refresh токен

                // Если пользователя с таким refresh токеном не найдено (т.е. странные ситуации, когда неавторизованный пользователь зачем-то хочет refresh токен)
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Некорректный refresh токен!");
                }
                else if (user.TokenExpired())
                {
                    throw new UnauthorizedAccessException("Срок действия refresh токена истек!");
                }

                var accessToken = CreateJWT(user);
                var newRefreshToken = new RefreshToken(30);

                // Устанавливаем новый refresh токен в бд
                user.RefreshToken = newRefreshToken.Token;
                user.TokenCreated = newRefreshToken.Created;
                user.TokenExpires = newRefreshToken.Expires;
                await _userRepository.Update(user);

                return new AuthTokensDTO(accessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                // тут можно реализовать логгирование
                throw; // перебрасываем исключение для того, чтобы его поймал middleware
            }
        }

        private AccessToken CreateJWT(User user)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:JwtSecret").Value!));

            var accessToken = new AccessToken(user, key, 30);

            return accessToken;
        }
    }
}
