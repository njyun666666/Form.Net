﻿using FormAPI.Errors;
using FormAPI.Models.Login;
using FormCore.Configuration;
using FormCore.Helpers;
using FormCore.Jwt;
using FormDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace FormAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : BaseController
{
	private readonly AppConfig _config;
	private readonly JwtHelper _jwtHelper;
	private readonly FormDbContext _context;

	public LoginController(AppConfig config, JwtHelper jwtHelper, FormDbContext context)
	{
		_config = config;
		_jwtHelper = jwtHelper;
		_context = context;
	}

	[HttpPost]
	public async Task<ActionResult<TokenViewModel>> Index(LoginModel login)
	{
		string encodingPW = EncodingHepler.ComputeHMACSHA256(login.Password, _config.APIKey());
		var user = await _context.TbOrgUsers.Include(x => x.TbOrgRoleUsers).FirstOrDefaultAsync(x => x.Email == login.Email && x.Enable && x.Password == encodingPW);

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

		var user = await _context.TbOrgUsers.Include(x => x.TbOrgRoleUsers).FirstOrDefaultAsync(x => x.Uid == tbRefresh.Uid && x.Enable);

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

		claims.Add(new Claim("photoUrl", user.PhotoUrl));

		if (user.TbOrgRoleUsers.Any(x => x.Rid == AppConst.Role.Administrator))
		{
			foreach (var rid in _context.TbOrgRoles.Select(x => x.Rid))
			{
				claims.Add(new Claim(ClaimTypes.Role, rid));
			}
		}
		else
		{
			foreach (var rid in user.TbOrgRoleUsers.Select(x => x.Rid).Distinct())
			{
				claims.Add(new Claim(ClaimTypes.Role, rid));
			}
		}

		string refresh_token = Guid.NewGuid().ToString();
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
