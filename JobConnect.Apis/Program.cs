using JobConnect.Core.Models;
using JobConnect.Core.Services;
using JobConnect.Repository.Data;
using JobConnect.Apis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobConnect.Services;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.Repository;
using JobConnect.Apis.IService;
using JobConnect.Apis.Services.JobService;

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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobRepository , JobRepository>();
builder.Services.AddScoped<IJobService , JobService>();
#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer();
#endregion

#region EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#endregion

var app = builder.Build();

// Migrate database and seed default user
#region Migration
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var _dbContext = services.GetRequiredService<AppDbContext>();

var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
	await _dbContext.Database.MigrateAsync();
	var userManager = services.GetRequiredService<UserManager<User>>();
	await AppDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
	var logger = loggerFactory.CreateLogger<Program>();
	logger.LogError(ex, "An error occurred while applying the migration");
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{
//	app.UseSwagger();
//	app.UseSwaggerUI(options =>
//	{
//		options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
//		options.RoutePrefix = string.Empty; // «Ã⁄· Swagger ›Ì «·„”«— «·—∆Ì”Ì («Œ Ì«—Ì)
//	});
//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

