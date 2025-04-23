using System.Security.Claims;
using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using AuthService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/a/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _dbContext;

    public AuthController(ITokenService tokenService, AppDbContext dbContext)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel? loginModel)
    {
        if (loginModel is null)
        {
            return BadRequest(Constants.BadRequestErrorMessage);
        }

        var user = _dbContext.LoginModels.FirstOrDefault(u =>
            u.UserName == loginModel.UserName && u.Password == loginModel.Password);

        if (user is null)
        {
            return Unauthorized(Constants.UnauthorizedErrorMessage);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, Constants.ManagerRole)
        };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        _dbContext.SaveChanges();

        return Ok(new AuthenticatedResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}