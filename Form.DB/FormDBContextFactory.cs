using FormCore.Configuration;
using FormDB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FormDB;

public class FormDBContextFactory : IDesignTimeDbContextFactory<FormDbContext>
{
	public FormDbContext CreateDbContext(string[] args)
	{
		var builder = new DbContextOptionsBuilder<FormDbContext>();
		var configuration = AppConfigurations.BuildConfiguration();
		builder.UseMySql(configuration.GetConnectionString("FormDB"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));

		return new FormDbContext(builder.Options);
	}
}
// Scaffold-DbContext Name=ConnectionStrings:FormDB Pomelo.EntityFrameworkCore.MySql -OutputDir Models -force -NoOnConfiguring
