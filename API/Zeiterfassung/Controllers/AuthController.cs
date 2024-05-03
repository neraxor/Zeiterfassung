using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Zeiterfassung.Data;
using Zeiterfassung.Data.DTO;
using Zeiterfassung.DTO;
using Zeiterfassung.Models;

namespace Zeiterfassung.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private static User _user = new User();
    private readonly IConfiguration _config;
    private readonly MyDbContext _context;

    public AuthController(IConfiguration configuration, MyDbContext context)
    {
        _config = configuration;
        _context = context;
    }

    /// <summary>
    /// allows a user to register an account
    /// </summary>
    /// <param name="request">request has userdata</param>
    /// <returns></returns>
    [HttpPost("register")]
    public ActionResult Register(UserRegisterDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }
        bool userExists = _context.Users.Any(s => s.Username == request.Username);
        if (!userExists)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            _user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Email
            };
            _context.Users.Add(_user);
            if (_context.SaveChanges() > 0)
            {
                return Ok("User created successfully");
            }
        }
        else
        {
            return BadRequest("User already exists");
        }
        return BadRequest("User could not be created");
    }
    /// <summary>
    /// login a user
    /// </summary>
    /// <param name="request">uses username and password</param>
    /// <returns>jwt token</returns>
    [HttpPost("login")]
    public ActionResult Login(UserLoginDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }
        User? user = _context.Users.FirstOrDefault(s => s.Username == request.Username);
        if (user is null)
        {
            return BadRequest("User not found");
        }
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Invalid password");
        }
        string token = CreateToken(user);
        return Ok(token);
    }

    /// <summary>
    /// generates a jwt token for a user to authenticate
    /// </summary>
    /// <param name="user">userdata</param>
    /// <returns>jwt token</returns>
    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        SymmetricSecurityKey key =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Token").Value!));
        SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            // expires: DateTime.Now.AddMinutes(1),
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}