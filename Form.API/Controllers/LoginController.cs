using FormCommon.Services;
using FormCore.Configuration;
using FormCore.Errors;
using FormCore.Helpers;
using FormCore.Jwt;
using FormCore.Models.Login;
using FormDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace FormAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController(AppConfig config, JwtHelper jwtHelper, FormDbContext context, UserService userService) : BaseController(userService)
{
	private readonly AppConfig _config = config;
	private readonly JwtHelper _jwtHelper = jwtHelper;
	private readonly FormDbContext _context = context;
	private readonly UserService _userService = userService;

	[HttpPost]
	public async Task<ActionResult<TokenViewModel>> Index(LoginModel login)
	{
		string encodingPW = EncodingHepler.ComputeHMACSHA256(login.Password, _config.APIKey());
		var user = await _context.TbOrgUsers.FirstOrDefaultAsync(x => x.Email == login.Email && x.Enable && x.Password == encodingPW);

		if (user == null)
		{
			throw new RestException(HttpStatusCode.Unauthorized, new { error = "Login Failed" });
		}

		var isDeptExists = await _context.TbOrgDeptUsers.AnyAsync(x => x.Uid == user.Uid && x.Enable);

		if (!isDeptExists)
		{
			throw new RestException(HttpStatusCode.Unauthorized, new { error = "No department set" });
		}

		var token = await CreateToken(user);

		await _context.SaveChangesAsync();
		return Ok(token);
	}

	[HttpPost(nameof(RefreshToken))]
	public async Task<ActionResult<TokenViewModel>> RefreshToken(RefreshTokenModel refreshToken)
	{
		var tbRefresh = await _context.TbRefreshTokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken.refresh_token);

		if (tbRefresh == null)
		{
			throw new RestException(HttpStatusCode.Unauthorized, new { error = "refresh token Failed" });
		}

		_context.TbRefreshTokens.Remove(tbRefresh);

		var user = await _context.TbOrgUsers.FirstOrDefaultAsync(x => x.Uid == tbRefresh.Uid && x.Enable);

		if (user == null)
		{
			await _context.SaveChangesAsync();
			throw new RestException(HttpStatusCode.Unauthorized, new { error = "refresh token Failed" });
		}

		var isDeptExists = await _context.TbOrgDeptUsers.AnyAsync(x => x.Uid == user.Uid && x.Enable);

		if (!isDeptExists)
		{
			await _context.SaveChangesAsync();
			throw new RestException(HttpStatusCode.Unauthorized, new { error = "refresh token Failed" });
		}

		var token = await CreateToken(user);

		await _context.SaveChangesAsync();
		return Ok(token);
	}

	private async Task<TokenViewModel> CreateToken(TbOrgUser user)
	{
		List<Claim> claims = [new Claim("uid", user.Uid)];

		if (!string.IsNullOrWhiteSpace(user.PhotoUrl))
		{
			claims.Add(new Claim("photoUrl", user.PhotoUrl));
		}

		var menuList = _context.TbMenus.Where(x => _context.FnAuth(user.Uid, x.MenuId)).Select(x => x.MenuId);
		foreach (var menuId in menuList)
		{
			claims.Add(new Claim(ClaimTypes.Role, menuId));
		}

		string refresh_token = EncodingHepler.NewID();
		await _context.TbRefreshTokens.AddAsync(new TbRefreshToken()
		{
			RefreshToken = refresh_token,
			ExpireTime = DateTime.Now.AddDays(7),
			Uid = user.Uid,
		});

		return new TokenViewModel()
		{
			Access_token = _jwtHelper.GenerateToken(user.Name, claims),
			Refresh_token = refresh_token
		};
	}
}
