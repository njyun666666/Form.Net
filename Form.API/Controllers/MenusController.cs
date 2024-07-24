﻿using AutoMapper;
using FormCore.Configurations;
using FormCore.Models.Menu;
using FormDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MenusController(FormDbContext context, IMapper mapper) : BaseController
{
	private readonly FormDbContext _context = context;
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
}
