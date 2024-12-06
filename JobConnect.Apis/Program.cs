using JobConnect.Core.Models;
using JobConnect.Core.Services;
using JobConnect.Repository.Data;
using JobConnect.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region DI
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITokenServices, TokenServices>();
#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); 
#endregion

var app = builder.Build();
#region Migration

using var Scope = app.Services.CreateScope();
var Services = Scope.ServiceProvider;
// Ask CLR for creating object from dbcontext explicitly
var _dbContext = Services.GetRequiredService<AppDbContext>();

var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();

try
{
	await _dbContext.Database.MigrateAsync();
	var UserManager = Services.GetRequiredService<UserManager<User>>();
	await AppDbContextSeed.SeedUserAsync(UserManager);
}
catch (Exception ex)
{
	var Logger = LoggerFactory.CreateLogger<Program>();
	Logger.LogError(ex, "An error occurred while applying the migration");
}
#endregion




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
