using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingAreas.Domain;
using ShoppingAreas.Domain.Interceptors;
using ShoppingAreas.Services;
using ShoppingAreas.Services.Interfaces;
using ShoppingAreas.Web.Extensions;
using ShoppingAreas.WebApi.Mappings;

namespace ShoppingAreas.WebApi
{
	public class Startup
	{
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

			// Auto Mapper Configurations
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new MappingProfile());
			});
			var mapper = mappingConfig.CreateMapper();
			services.AddSingleton(mapper);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
