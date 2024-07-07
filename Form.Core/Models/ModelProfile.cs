using AutoMapper;
using FormCore.Models.Menu;
using FormDB.Models;

namespace FormCore.Models;

public class ModelProfile : Profile
{
	public ModelProfile()
	{
		CreateMap<TbMenu, MenuViewModel>();
		CreateMap<MenuViewModel, AuthMenuViewModel>();
	}
}
