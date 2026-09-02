using HtawPyi.Domain;

namespace HtawPyi.Application;

public class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    ITokenService tokens,
    IUnitOfWork uow,
    TimeProvider clock)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!email.Contains('@') || email.Length < 5)
            throw new DomainException("A valid email is required.");
        if (request.Password.Length < 8)
            throw new DomainException("Password must be at least 8 characters.");
        if (await users.FindByEmailAsync(email, ct) is not null)
            throw new DomainException("An account with this email already exists.", 409);

        var user = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(request.Password),
            Role = UserRole.Customer,
            CreatedAt = clock.GetUtcNow().UtcDateTime
        };
        await users.AddAsync(user, ct);
        var response = await IssueTokensAsync(user, ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid email or password.", 401);

        var response = await IssueTokensAsync(user, ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var stored = await refreshTokens.FindByHashAsync(tokens.HashRefreshToken(request.RefreshToken), ct);
        if (stored is null || !stored.IsActive(now) || stored.User is null)
            throw new DomainException("Invalid refresh token.", 401);

        // Rotation: the presented token is spent regardless of what follows.
        stored.RevokedAt = now;
        var response = await IssueTokensAsync(stored.User, ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
    {
        var (refreshToken, hash, expiresAt) = tokens.CreateRefreshToken();
        await refreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            User = user,
            TokenHash = hash,
            ExpiresAt = expiresAt,
            CreatedAt = clock.GetUtcNow().UtcDateTime
        }, ct);
        return new AuthResponse(
            tokens.CreateAccessToken(user), refreshToken, user.Email, user.Role.ToString());
    }
}
