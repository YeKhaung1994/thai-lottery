using System.Security.Cryptography;
using System.Text;
using HtawPyi.Application;
using HtawPyi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------- Options
var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("Jwt").Bind(jwtOptions);
if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    // No key configured: generate an ephemeral one (dev only). Nothing is
    // persisted; tokens become invalid when the process exits.
    jwtOptions.Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException(
            "Jwt:Key must be configured outside Development.");
}

var paymentOptions = new PaymentOptions();
builder.Configuration.GetSection("Payment").Bind(paymentOptions);
// 2C2P config comes from env: PAYMENT__2C2P__MERCHANT_ID / SECRET_KEY.
paymentOptions.TwoCTwoP.MerchantId =
    builder.Configuration["PAYMENT:2C2P:MERCHANT_ID"] ?? paymentOptions.TwoCTwoP.MerchantId;
paymentOptions.TwoCTwoP.SecretKey =
    builder.Configuration["PAYMENT:2C2P:SECRET_KEY"] ?? paymentOptions.TwoCTwoP.SecretKey;
if (paymentOptions.Provider == "TwoCTwoP" &&
    (string.IsNullOrWhiteSpace(paymentOptions.TwoCTwoP.MerchantId) ||
     string.IsNullOrWhiteSpace(paymentOptions.TwoCTwoP.SecretKey)))
    throw new InvalidOperationException(
        "Payment provider is TwoCTwoP but PAYMENT__2C2P__MERCHANT_ID / " +
        "PAYMENT__2C2P__SECRET_KEY are blank. Fill them or use Provider=Mock.");

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "ConnectionStrings__Default is blank. Set it in the environment " +
        "(see db/.env) before starting the API.");

// ------------------------------------------------------------ Services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton(paymentOptions);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IDrawResultRepository, DrawResultRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddHttpClient<IGloClient, GloClient>(c =>
    c.BaseAddress = new Uri(GloClient.BaseUrl));

if (paymentOptions.Provider == "TwoCTwoP")
    builder.Services.AddHttpClient<IPaymentProvider, TwoCTwoPProvider>();
else
    builder.Services.AddSingleton<IPaymentProvider, MockPaymentProvider>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AdminTicketService>();
builder.Services.AddSingleton(new HtawPyi.Api.PaymentOptionsView(paymentOptions.Provider));

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ClockSkew = TimeSpan.FromSeconds(30)
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(
        paymentOptions.CustomerAppUrl,
        builder.Configuration["AdminAppUrl"] ?? "http://localhost:8081")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// DomainException -> problem+json with its status code.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (DomainException ex)
    {
        context.Response.StatusCode = ex.StatusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            type = "about:blank",
            title = ex.Message,
            status = ex.StatusCode
        });
    }
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
