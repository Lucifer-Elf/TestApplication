using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servize.Domain.Mapper;
using Servize.Domain.Repositories;
using AutoMapper;
namespace Servize
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

       
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.Audience = Configuration["AAD:ResourceId"];
                    opt.Authority = $"{Configuration["ADD:Instance"]}{Configuration["ADD:TenantId"]}";
                });
            services.AddDbContext<ServizeDBContext>(options => options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=TestDatabase;Trusted_Connection=True;"));
            services.AddScoped<ServizeProviderRespository>();
            services.AddControllers();

            var config = new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new MapperSetting());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        
        }

     
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();   // add to pipline 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
              //  endpoints.mapServiceBaseEndpoints("", "");
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
