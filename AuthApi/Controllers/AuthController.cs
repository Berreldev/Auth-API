using AuthApi.Data;
using AuthApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    // 1) Criar usuário
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(req.FullName) ||
            string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Nenhum campo pode estar vazio.");

        if (req.Password.Length < 8)
            return BadRequest("Senha deve ter pelo menos 8 caracteres.");

        bool exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists)
            return BadRequest("E-mail já cadastrado.");

        var user = new User
        {
            FullName = req.FullName.Trim(),
            Email = req.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok($"Usuario {user.FullName} criado com sucesso");
    }

    // 2) Login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Nenhum campo pode estar vazio.");

        if (req.Password.Length < 8)
            return BadRequest("Senha deve ter pelo menos 8 caracteres.");

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Credenciais inválidas");

        string token = GenerateJwt(user);

        return Ok(new
        {
            status = $"Usuario {user.FullName} logado com sucesso",
            authToken = token
        });
    }

    // 3) Verificar se usuário está logado
    [HttpPost("check")]
    public IActionResult Check([FromBody] TokenRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.AuthToken))
            return BadRequest("authToken não pode estar vazio.");

        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(req.AuthToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _cfg["Jwt:Issuer"],
                    ValidAudience = _cfg["Jwt:Audience"],
                    ValidateLifetime = true
                },
                out _);

            return Ok(new { isLogged = true });
        }
        catch
        {
            return Ok(new { isLogged = false });
        }
    }

    // Helper para gerar o JWT
    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim("fullName", user.FullName),
            new Claim("email", user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                int.Parse(_cfg["Jwt:ExpireHours"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}