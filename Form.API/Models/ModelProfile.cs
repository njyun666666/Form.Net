using AutoMapper;
using FormAPI.Models.Menu;
using FormDB.Models;

namespace FormAPI.Models;

public class ModelProfile : Profile
{
	public ModelProfile()
	{
		CreateMap<TbMenu, MenuViewModel>();
		CreateMap<MenuViewModel, AuthMenuViewModel>();
	}
}
