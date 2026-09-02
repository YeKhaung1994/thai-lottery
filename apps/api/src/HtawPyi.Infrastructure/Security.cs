using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HtawPyi.Application;
using HtawPyi.Domain;
using Microsoft.IdentityModel.Tokens;

namespace HtawPyi.Infrastructure;

/// <summary>PBKDF2 (SHA-256, 100k iterations). No external dependencies.</summary>
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"pbkdf2|{Iterations}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string stored)
    {
        var parts = stored.Split('|');
        if (parts.Length != 4 || parts[0] != "pbkdf2") return false;
        if (!int.TryParse(parts[1], out var iterations)) return false;
        var salt = Convert.FromBase64String(parts[2]);
        var expected = Convert.FromBase64String(parts[3]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public class JwtOptions
{
    /// <summary>HMAC signing key. Blank in config: the API generates an
    /// ephemeral key at startup (dev only — tokens die with the process).</summary>
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "htawpyi";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}

public class JwtTokenService(JwtOptions options, TimeProvider clock) : ITokenService
{
    public string CreateAccessToken(User user)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Issuer,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ],
            notBefore: now,
            expires: now.AddMinutes(options.AccessTokenMinutes),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Token, string Hash, DateTime ExpiresAt) CreateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        return (token, HashRefreshToken(token),
            clock.GetUtcNow().UtcDateTime.AddDays(options.RefreshTokenDays));
    }

    public string HashRefreshToken(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
