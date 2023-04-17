using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTOs;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountController : BaseController
    {
        public readonly IUnitOfWork unitOfWork;
        private readonly string expireTime;
        private readonly string secretKey;

        public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.expireTime = configuration.GetSection("AppSettings:TokenExpireInMinutes").Value.ToString();
            this.secretKey = configuration.GetSection("AppSettings:SecretKey").Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDTO loginReqDTO)
        {
            var user = await unitOfWork.UserRepository.Authenticate(loginReqDTO.UserName, loginReqDTO.Password);

            if (user == null) { return Unauthorized(); }

            var loginRes = new LoginResDTO() { UserName = user.UserName, Token = CreateJWT(user), Expires = string.Concat(this.expireTime," (mins)")  };

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
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials= signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
