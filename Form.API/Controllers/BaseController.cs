﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FormAPI.Controllers;

[Controller]
public class BaseController : ControllerBase
{
	public string _uid => GetUserClaim("uid").FirstOrDefault();
	public List<string> _roles => GetUserClaim(ClaimTypes.Role);

	[NonAction]
	public List<string> GetUserClaim(string ClaimName)
	{
		var user = HttpContext.User;

		if (user.HasClaim(c => c.Type == ClaimName))
		{
			var v = user.Claims.Where(c => c.Type == ClaimName).Select(x => x.Value).ToList();
			if (v != null)
			{
				return v;
			}
		}

		return [];
	}

	[NonAction]
	public bool IsRole(string role)
	{
		return _roles.Contains(role);
	}
}

