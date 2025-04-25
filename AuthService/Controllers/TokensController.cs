using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace AuthService.Controllers;

[Route("api/a/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public TokensController(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }


    [Route("refresh")]
    [HttpPost]
    public IActionResult Refresh(TokenApiModel? tokenApiModel)
    {
        if (tokenApiModel is null)
        {
            return BadRequest(GlobalConstants.BadRequestErrorMessage);
        }

        var accessToken = tokenApiModel.AccessToken;
        var refreshToken = tokenApiModel.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!);
        var userName = principal.Identity!.Name;

        var user = _dbContext.LoginModels.FirstOrDefault(u => u.UserName == userName);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest(GlobalConstants.BadRequestErrorMessage);
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

        _dbContext.SaveChanges();

        return Ok(new AuthenticatedResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [Route("revoke")]
    [HttpPost]
    [Authorize]
    public IActionResult RevokeToken()
    {
        var userName = User.Identity!.Name;
        var user = _dbContext.LoginModels.FirstOrDefault(u => u.UserName == userName);

        if (user is null)
        {
            return BadRequest(GlobalConstants.BadRequestErrorMessage);
        }

        user.RefreshToken = null;
        _dbContext.SaveChanges();

        return NoContent();
    }
}