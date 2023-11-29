using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScheduleABMK.Application.Auth;
using ScheduleABMK.Data.Repositories.Interfaces;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ScheduleABMK.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet, Authorize(Roles = "Student")]
        public ActionResult<string> GetMe()
        {
            return Ok("gdfgdfgdfgdfgdfgdfg");
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDTO request)
        {
            // Добавить атрибуты для UserDTO
            if (ModelState.IsValid)
            {
                User? user = await _userRepository.GetByEmail(request.Email);
                if (user == null)
                {
                    CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
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
                    return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
                }
                else
                {
                    return BadRequest("User with this email is already exists");
                }
            }
            return BadRequest("Uncorrect data");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDTO request)
        {
            User? user = await _userRepository.GetByEmail(request.Email);
            if (user == null)
                return BadRequest("User not found");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Wrong password");

            string token = CreateToken(user);

            var refreshToken = GenerateRefreshToken();

            // Устанавливаем refresh token для клиента в куки
            SetRefreshToken(refreshToken);

            // При каждой авторизации пользователя обновляем ему refresh token в бд
            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;
            await _userRepository.Update(user);

            return Ok(token);
        }

        // Этот метод мы вызываем, когда клиент понял, что истекает или уже истек срок действия access токена и надо выдать новую пару acccess и refresh токена
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userRepository.GetByRefreshToken(refreshToken); // P.S. наверное надо выставить индексы на refresh токен

            // Если пользователя с таким refresh токеном не найдено (т.е. странные ситуации, когда неавторизованный пользователь зачем-то хочет refresh токен)
            if (user == null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired");
            }

            string token = CreateToken(user);

            // Устанавливаем новый refresh токен в куки
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);
            // Устанавливаем новый refresh токен в бд
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            await _userRepository.Update(user);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                // HttpOnly = true для того, чтобы в клиенте невозможно было получить refresh token через скрипты js
                HttpOnly = true,
                Expires = refreshToken.Expires,
                SameSite = SameSiteMode.None,
                Secure = true // Важно для HTTPS
            };

            // Добавляем cookie с именем refreshToken
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private string CreateToken(User user)
        {
            // в claims будет лежать вся информация о пользователе, которая нам нужна в токене
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:JwtSecret").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(10), // токен доступен 5 сек
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
