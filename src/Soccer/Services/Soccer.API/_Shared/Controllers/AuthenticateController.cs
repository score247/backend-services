using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Soccer.API.Shared.Configurations;
using Soccer.Core._Shared.Helpers;
using Soccer.Core.Shared.Models;

namespace Soccer.API._Shared.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private const int ExpiredTokenSeconds = 600;
        private readonly IAppSettings appSettings;
        private readonly ILogger logger;
        private readonly ICryptographyHelper cryptographyHelper;

        public AuthenticateController(
            IAppSettings appSettings,
            ILogger logger,
            ICryptographyHelper cryptographyHelper)
        {
            this.appSettings = appSettings;
            this.logger = logger;
            this.cryptographyHelper = cryptographyHelper;
        }

        [Route("generateToken")]
        [AllowAnonymous, HttpGet]
        public async Task<string> GenerateToken(string userId, string encryptedInfo)
            => await CheckAndGenerateToken(userId, encryptedInfo);

        [Route("generateToken")]
        [AllowAnonymous, HttpPost]
        public async Task<string> GenerateTokenPost([FromBody]AuthenticateInfo authenticateInfo)
            => await CheckAndGenerateToken(authenticateInfo?.UserId, authenticateInfo?.EncryptedInfo);

        private async Task<string> CheckAndGenerateToken(string userId, string encryptedInfo)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return userId;
            }

            try
            {
                var decryptedUserId = cryptographyHelper.Decrypt(encryptedInfo, appSettings.EncryptKey);

                if (decryptedUserId != userId)
                {
                    await logger.InfoAsync($"Failed decrypted user {userId}, decrypted user {decryptedUserId}");
                }
            }
            catch (Exception ex)
            {
                await logger.InfoAsync($"Failed decrypted user {userId}, encrypted info {encryptedInfo} {ex.ToString()}");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddSeconds(ExpiredTokenSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            await logger.InfoAsync($"Grant Token for user {userId}, token {token}");

            return token;
        }
    }
}