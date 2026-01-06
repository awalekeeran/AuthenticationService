using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTOs;
using WebAPI.Interfaces;
using WebAPI.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace WebAPI.Controllers
{
    public class AccountController : BaseController
    {
        public readonly IUnitOfWork unitOfWork;
        private readonly int expireTimeInMinutes;
        private readonly int refreshTokenExpireInDays;
        private readonly string secretKey;

        public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.expireTimeInMinutes = int.Parse(configuration.GetSection("AppSettings:TokenExpireInMinutes").Value ?? "30");
            this.refreshTokenExpireInDays = int.Parse(configuration.GetSection("AppSettings:RefreshTokenExpireInDays").Value ?? "7");
            this.secretKey = configuration.GetSection("AppSettings:SecretKey").Value;
        }

        [HttpPost("login")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(LoginReqDTO loginReqDTO)
        {
            var user = await unitOfWork.UserRepository.Authenticate(loginReqDTO.UserName, loginReqDTO.Password);

            if (user == null) { return Unauthorized(); }

            // Generate new refresh token
            var refreshToken = unitOfWork.RefreshTokenGeneratorRepository.GenerateToken(user.ID);

            var loginRes = new LoginResDTO() 
            { 
                UserName = user.UserName, 
                Token = CreateJWT(user), 
                Expires = string.Concat(this.expireTimeInMinutes, " (mins)"),
                RefreshToken = refreshToken
            };

            return Ok(loginRes);
        }

        private string CreateJWT(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.secretKey));

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

            var signingCredentials = new SigningCredentials(
                key,SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(this.expireTimeInMinutes),
                SigningCredentials= signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required");
            }

            // Validate the refresh token from the database
            var user = unitOfWork.UserRepository.GetUserByRefreshToken(request.RefreshToken);
            
            if (user == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            // Generate new JWT token
            var newJwtToken = CreateJWT(user);

            // Rotate the refresh token (generate a new one)
            var newRefreshToken = unitOfWork.RefreshTokenGeneratorRepository.GenerateToken(user.ID);

            var response = new LoginResDTO()
            {
                UserName = user.UserName,
                Token = newJwtToken,
                Expires = string.Concat(this.expireTimeInMinutes, " (mins)"),
                RefreshToken = newRefreshToken
            };

            return Ok(response);
        }
    }
}
