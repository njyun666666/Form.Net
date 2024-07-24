using Microsoft.EntityFrameworkCore;

namespace FormDB.Models;
public partial class FormDbContext : DbContext
{
	partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDbFunction(() => FnAuth(default, default))
			.HasName("FnAuth");
	}

	public bool FnAuth(string in_Uid, string in_MenuId) => throw new NotImplementedException();
}
