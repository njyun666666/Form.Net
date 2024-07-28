using AutoMapper;
using FormCommon.Services;
using FormCore.Configurations;
using FormCore.Helpers;
using FormCore.Models.Menu;
using FormDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MenusController(FormDbContext context, IMapper mapper, UserService userService) : BaseController(userService)
{
	private readonly FormDbContext _context = context;
	private readonly UserService _userService = userService;
	private readonly IMapper _mapper = mapper;
	List<TbMenu> _menus = null!;

	// GET: api/Menus
	[HttpGet]
	public async Task<ActionResult<List<MenuViewModel>>> GetTbMenus()
	{
		_menus = await _context.TbMenus.Where(x => x.Enable && _context.FnAuth(_uid, x.MenuId)).OrderBy(m => m.Sort).ToListAsync();
		return SetMenu(null);
	}

	// GET: api/Menus/AuthMenus
	[HttpGet(nameof(AuthMenus))]
	[Authorize(Roles = AppConst.MenuId.AuthMenu)]
	public async Task<ActionResult<List<AuthMenuViewModel>>> AuthMenus()
	{
		_menus = await _context.TbMenus.OrderBy(m => m.Sort).ToListAsync();
		return _mapper.Map<List<AuthMenuViewModel>>(SetMenu(null));
	}

	[HttpPost(nameof(AuthMenus))]
	[Authorize(Roles = AppConst.MenuId.AuthMenu)]
	public async Task<ActionResult> AuthMenus(TbMenu model)
	{
		// Add
		if (string.IsNullOrWhiteSpace(model.MenuId) || !TbMenusExists(model.MenuId))
		{
			model.MenuId = EncodingHepler.NewID();
			_context.TbMenus.Add(model);
		}
		else
		{
			var targetMenu = await _context.TbMenus.FirstOrDefaultAsync(x => x.MenuId == model.MenuId);

			if (targetMenu == null)
			{
				return NotFound();
			}

			_context.Entry(targetMenu).CurrentValues.SetValues(model);
		}

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{
			return Conflict();
		}

		return NoContent();
	}

	[HttpPost(nameof(AuthMenusSort))]
	[Authorize(Roles = AppConst.MenuId.AuthMenu)]
	public async Task<ActionResult> AuthMenusSort(MenuSortModel model)
	{
		var list = await _context.TbMenus.Where(x => model.MenuIds.Contains(x.MenuId)).ToListAsync();

		for (int i = 0; i < model.MenuIds.Count; i++)
		{
			var menu = list.FirstOrDefault(x => x.MenuId == model.MenuIds[i]);
			if (menu != null)
			{
				menu.Sort = i;
			}
		}

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{
			return Conflict();
		}

		return NoContent();
	}
	private List<MenuViewModel> SetMenu(string? parentMenuID)
	{
		IEnumerable<TbMenu> menuList;

		if (string.IsNullOrWhiteSpace(parentMenuID))
		{
			menuList = _menus.Where(x => x.ParentMenuId == null || x.ParentMenuId == "");
		}
		else
		{
			menuList = _menus.Where(x => x.ParentMenuId == parentMenuID);
		}

		if (!menuList.Any())
		{
			return null;
		}

		var menus = _mapper.Map<List<MenuViewModel>>(menuList);

		menus.ForEach(m =>
		{
			m.Children = SetMenu(m.MenuId);
		});

		return menus;
	}

	private bool TbMenusExists(string id)
	{
		return (_context.TbMenus?.Any(e => e.MenuId == id)).GetValueOrDefault();
	}
}
