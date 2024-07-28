using FormCommon.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormAPI.Controllers;

[Controller]
public class BaseController(UserService userService) : ControllerBase
{
	private readonly UserService _userService = userService;

	public string _uid => _userService.Uid;

	public List<string> _roles => _userService.Roles;
}
