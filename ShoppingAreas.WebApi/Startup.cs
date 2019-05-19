using System;
using System.Diagnostics;
using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShoppingAreas.Domain;
using ShoppingAreas.Domain.Interceptors;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.Services;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Web.Extensions;
using ShoppingAreas.WebApi.Filters;
using ShoppingAreas.WebApi.Helpers;
using ShoppingAreas.WebApi.Mappings;
using ShoppingAreas.WebApi.Models;
using ShoppingAreas.WebApi.Services.Auth;
using ShoppingAreas.WebApi.Services.Auth.Requirements;
using Swashbuckle.AspNetCore.Swagger;

namespace ShoppingAreas.WebApi
{
	public class Startup
	{
		private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; //	todo: get this from	somewhere secure
		private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificMethods",
					option =>
					{
						option.WithOrigins("http://localhost:4200")
							.AllowAnyMethod()
							.AllowCredentials()
							.AllowAnyHeader();
					});
			});

			var dbConnectionString = Configuration.GetConnectionString("DbConnection");
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseMySql(dbConnectionString, x => x.MigrationsHistoryTable("_migrations")));

			// Interception
			DiagnosticListener.AllListeners.Subscribe(new GlobalListener());

			// Add application services.
			services.AddTransient<IAreaService, AreaService>();
			services.AddTransient<IEquipmentService, EquipmentService>();
			services.AddTransient<IProductService, ProductService>();
			services.AddTransient<IReportsService, ReportsService>();

			ConfigureAuth(services);

			// Adds	a default in-memory	implementation of IDistributedCache.
			services.AddDistributedMemoryCache();

			// Auto Mapper Configurations
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new MappingProfile());
			});
			var mapper = mappingConfig.CreateMapper();
			services.AddSingleton(mapper);

			services.AddMvc(options =>
				{
					options.Filters.Add(typeof(ValidateModelStateAttribute));
				})
				.AddFluentValidation(fv =>
				{
					fv.RegisterValidatorsFromAssemblyContaining<Startup>();
					fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseCors("AllowSpecificMethods");
			app.UseCustomExceptionHandler();

			if (env.IsDevelopment())
			{
				// app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.RoutePrefix = string.Empty;
			});

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseMvc();
		}

		#region Auth
		
		private void ConfigureAuth(IServiceCollection services)
		{
			services.AddSingleton<IJwtFactory, JwtFactory>();

			// jwt wire	up
			// Get options from	app	settings
			var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

			// Configure JwtIssuerOptions
			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
				options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
			});

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
				ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _signingKey,

				RequireExpirationTime = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			}).AddJwtBearer(configureOptions =>
			{
				configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				configureOptions.TokenValidationParameters = tokenValidationParameters;
				configureOptions.SaveToken = true;
			});

			// api user	claim policy
			services.AddAuthorization(options =>
			{
				options.AddPolicy("ApiAdmin", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAdmin));
				options.AddPolicy("ApiUser", policy => policy.AddRequirements(new IsAdminOrUserRequirement()));
			});

			// add identity
			var builder = services.AddIdentityCore<User>(o =>
			{
				// configure identity options
				o.Password.RequireDigit = false;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 6;
			});
			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole<long>), builder.Services);
			builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
		}

		#endregion
	}
}
