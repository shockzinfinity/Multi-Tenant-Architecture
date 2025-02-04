using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TenantService;

namespace UserPortalService
{
  public class Constants
  {
    public const string Authority = "http://localhost:5000";
    public const string ApiResourceName = "api.portfolio.manager.v1";
  }

  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment env)
    {
      // In ASP.NET Core 3.0 `env` will be an IWebHostEnvironment, not IHostingEnvironment.
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      this.Configuration = builder.Build();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.RegisterRedis();
      services.AddControllers();
      services.AddAutoMapper(typeof(Startup));
      services.AddHttpContextAccessor();
      services.RegisterDbDependancies();
      services.RegisterServiceDependancies();

      services.AddAuthentication("Bearer")
          .AddJwtBearer("Bearer", options =>
          {
            options.Authority = Constants.Authority;
            options.RequireHttpsMetadata = false;
            // ApiResourceName
            options.Audience = Constants.ApiResourceName;
          });

      services.AddCors(options =>
      {
        options.AddPolicy("corspolicy",
                      builder =>
                      {
                        builder.AllowAnyOrigin()//(new string[] { "*", "http://localhost:4200" })
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                      });
      });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserPortalService", Version = "v1" });
        c.OperationFilter<AddRequiredHeaderParameter>();
      });
      services.AddOptions();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserPortalService v1"));
      }

      app.UseRouting();
      app.UseCors("corspolicy");
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}