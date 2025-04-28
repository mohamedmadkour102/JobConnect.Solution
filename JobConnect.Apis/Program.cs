#region OLDCODE
//using JobConnect.Core.Models;
//using JobConnect.Core.Services;
//using JobConnect.Repository.Data;
//using JobConnect.Apis;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using JobConnect.Services;
//using JobConnect.Apis.IRepository;
//using JobConnect.Apis.Repository;
//using JobConnect.Apis.IService;
//using JobConnect.Apis.Services.JobService;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//#region DI
//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

//builder.Services.AddScoped<ITokenServices, TokenServices>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IJobRepository , JobRepository>();
//builder.Services.AddScoped<IJobService , JobService>();
//#endregion

//#region Identity
//builder.Services.AddIdentity<User, IdentityRole>()
//	.AddEntityFrameworkStores<AppDbContext>()
//	.AddDefaultTokenProviders();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer();
//#endregion

//#region EmailSettings
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//#endregion

//var app = builder.Build();

//// Migrate database and seed default user
//#region Migration
//using var scope = app.Services.CreateScope();
//var services = scope.ServiceProvider;
//var _dbContext = services.GetRequiredService<AppDbContext>();

//var loggerFactory = services.GetRequiredService<ILoggerFactory>();

//try
//{
//	await _dbContext.Database.MigrateAsync();
//	var userManager = services.GetRequiredService<UserManager<User>>();
//	await AppDbContextSeed.SeedUserAsync(userManager);
//}
//catch (Exception ex)
//{
//	var logger = loggerFactory.CreateLogger<Program>();
//	logger.LogError(ex, "An error occurred while applying the migration");
//}
//#endregion

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//	app.UseSwagger();
//	app.UseSwaggerUI();
//}



//app.UseHttpsRedirection();


//app.UseAuthorization();

//app.MapControllers();

//app.Run(); 
#endregion

//using JobConnect.Core.Models;
//using JobConnect.Core.Services;
//using JobConnect.Repository.Data;
//using JobConnect.Apis;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using JobConnect.Services;
//using JobConnect.Apis.IRepository;
//using JobConnect.Apis.Repository;
//using JobConnect.Apis.IService;
//using JobConnect.Apis.Services.JobService;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//#region DI
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

//builder.Services.AddScoped<ITokenServices, TokenServices>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IJobRepository, JobRepository>();
//builder.Services.AddScoped<IJobService, JobService>();
//#endregion

//#region Identity
//builder.Services.AddIdentity<User, IdentityRole>()
//	.AddEntityFrameworkStores<AppDbContext>()
//	.AddDefaultTokenProviders();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer();
//#endregion

//#region EmailSettings
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//#endregion

//// CORS Configuration
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowFrontend",
//		policy =>
//		{
//			policy.WithOrigins("http://localhost:5173") 
//				  .AllowAnyMethod() 
//				  .AllowAnyHeader() 
//				  .AllowCredentials(); 
//		});
//});

//var app = builder.Build();

//// Migrate database and seed default user
//#region Migration
//using var scope = app.Services.CreateScope();
//var services = scope.ServiceProvider;
//var _dbContext = services.GetRequiredService<AppDbContext>();
//var loggerFactory = services.GetRequiredService<ILoggerFactory>();

//try
//{
//	await _dbContext.Database.MigrateAsync();
//	var userManager = services.GetRequiredService<UserManager<User>>();
//	await AppDbContextSeed.SeedUserAsync(userManager);
//}
//catch (Exception ex)
//{
//	var logger = loggerFactory.CreateLogger<Program>();
//	logger.LogError(ex, "An error occurred while applying the migration");
//}
//#endregion

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//	app.UseSwagger();
//	app.UseSwaggerUI();
//}

//// Use CORS Policy
//app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();


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

using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JobConnect.Apis.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
//builder.Services.AddSwaggerGen(c =>
//{
//	c.SwaggerDoc("v1", new OpenApiInfo
//	{
//		Title = "JobConnect API",
//		Version = "v1"
//	});


//	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//	{
//		Name = "Authorization",
//		Type = SecuritySchemeType.Http,
//		Scheme = "bearer",
//		BearerFormat = "JWT",
//		In = ParameterLocation.Header,
//		Description = "Enter your JWT token with the Bearer prefix. Example: Bearer {your token}"
//	});


//	c.AddSecurityRequirement(new OpenApiSecurityRequirement()
//	{
//		{
//			new OpenApiSecurityScheme
//			{
//				Reference = new OpenApiReference
//				{
//					Type = ReferenceType.SecurityScheme,
//					Id = "Bearer"
//				},
//				Scheme = "Bearer",
//				Name = "Authorization",
//				In = ParameterLocation.Header,
//			},
//			new List<string>()
//		}
//	});
//});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobConnect API", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter your JWT token in the format: Bearer {token}"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
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



builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IJobRepository, JobRepository>();
//builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IEmployerService , EmployerService>();
builder.Services.AddScoped<IEmployerRepository, EmployerRepository>();
builder.Services.AddScoped<IJobSeekerRepository, JobSeekerRepository>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();


#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer(options =>
//	{
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidateLifetime = true,
//			ValidateIssuerSigningKey = true,
//			ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
//			ValidAudience = builder.Configuration["JWT:ValidAudience"],
//			IssuerSigningKey = new SymmetricSecurityKey(
//				Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
//		};
//	});


// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

#endregion

#region EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#endregion

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend",
		policy =>
		{
			policy.WithOrigins("http://localhost:5173")
				  .AllowAnyMethod()
				  .AllowAnyHeader()
				  .AllowCredentials();
		});
});

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
app.UseStaticFiles();
app.UseHttpsRedirection();
//app.UseCors("AllowFrontend");
app.UseAuthentication(); 
app.UseAuthorization();


app.MapControllers();
app.Run();
