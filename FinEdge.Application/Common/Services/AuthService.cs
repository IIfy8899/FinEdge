using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Exceptions;
using FinEdge.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinEdge.Application.Common.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration) : IAuthService
{
    public async Task<string?> Authenticate(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email)
            ?? throw new NotFoundException(nameof(User), email);

        if (!VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
