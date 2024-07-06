using FormAPI.Errors;
using FormAPI.Models;
using FormCore.Configuration;
using FormCore.Jwt;
using FormDB.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer",
	new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization"
	});

	options.AddSecurityRequirement(
	new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});

});

builder.Services.AddDbContext<FormDbContext>(options =>
{
#if DEBUG
	string FormDB = builder.Configuration.GetConnectionString("FormDB");
#else
	string FormDB = FormCore.Helpers.AccessSecretVersion.Get(builder.Configuration["ProjectID"], "FormDB");
#endif

	options.UseMySql(FormDB, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));
});

builder.Services.AddSingleton<AppConfig>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.IncludeErrorDetails = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = false,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SignKey")))
		};
	});

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(ModelProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
