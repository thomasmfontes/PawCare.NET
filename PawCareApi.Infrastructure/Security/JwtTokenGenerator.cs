using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime Expiration) GenerateToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? "MinhaChaveSuperSecretaParaPawCareSprint4Fiap2026_AssinaturaJWT_123456789";
            var issuer = _configuration["Jwt:Issuer"] ?? "PawCareApi";
            var audience = _configuration["Jwt:Audience"] ?? "PawCareApi_Clients";
            var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 4;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var expiration = DateTime.UtcNow.AddHours(expirationHours);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.GivenName, user.Nome),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("RM", "562254")
                }),
                Expires = expiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiration);
        }
    }
}

