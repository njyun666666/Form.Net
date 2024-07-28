using FormCommon.Services;
using Microsoft.EntityFrameworkCore;

namespace FormDB.Models;
public partial class FormDbContext : DbContext
{
	private readonly UserService _userService;
	public FormDbContext(DbContextOptions<FormDbContext> options, UserService userService)
	: base(options)
	{
		_userService = userService;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries())
		{
			if (entry.State == EntityState.Modified)
			{
				entry.CurrentValues.SetValues(new
				{
					LastModifiedTime = DateTime.Now,
					LastModifiedBy = _userService.Uid,
				});
			}
		}

		return await base.SaveChangesAsync(cancellationToken);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDbFunction(() => FnAuth(default, default))
			.HasName("FnAuth");
	}

	public bool FnAuth(string in_Uid, string in_MenuId) => throw new NotImplementedException();
}
