using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FormCommon.Services;
public class UserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public string Uid => GetUserClaim("uid").FirstOrDefault();

	public List<string> Roles => GetUserClaim(ClaimTypes.Role);

	public List<string> GetUserClaim(string ClaimName)
	{
		var user = _httpContextAccessor.HttpContext.User;

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

	public bool IsRole(string role)
	{
		return Roles.Contains(role);
	}
}
