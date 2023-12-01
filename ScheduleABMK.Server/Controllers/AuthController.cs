using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScheduleABMK.Application.Auth;
using ScheduleABMK.Data.Repositories.Interfaces;
using ScheduleABMK.Domain;
using ScheduleABMK.Server.Models;
using ScheduleABMK.Server.Services.Interfaces;
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
        private PasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, IRoleRepository roleRepository, PasswordHasher passwordHasher, IAuthService authService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        [HttpGet, Authorize(Roles = "Student")]
        public ActionResult<string> GetMe()
        {
            return Ok("gdfgdfgdfgdfgdfgdfg");
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDTO request)
        {
            var registeredUser = await _authService.Register(request);

            return CreatedAtAction(nameof(Register), new { id = registeredUser.Id }, registeredUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDTO request)
        {
            var authTokens = await _authService.Login(request);
            SetRefreshTokenToCookies(authTokens.RefreshToken);
            var accessToken = authTokens.AccessToken.Token;

            return Ok(accessToken);
        }

        // Этот метод мы вызываем, когда клиент понял, что истекает или уже истек срок действия access токена и надо выдать новую пару acccess и refresh токена
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var newAuthTokens = await _authService.UpdateRefreshToken(refreshToken);
            SetRefreshTokenToCookies(newAuthTokens.RefreshToken);
            var accessToken = newAuthTokens.AccessToken.Token;

            return Ok(accessToken);
        }

        private void SetRefreshTokenToCookies(RefreshToken refreshToken)
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
    }
}
