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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Servize.Authentication;
using Microsoft.AspNetCore.Identity;
using Serilog;

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
            services.AddControllers(); // controller Registered
            services.AddScoped<ServizeProviderRespository>();
            services.AddScoped<Utility.Utilities>();

            //EnitiyFrameWork
            services.AddDbContext<ServizeDBContext>(options => options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=TestDatabase;Trusted_Connection=True;"));

            //Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ServizeDBContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddTokenProvider("ServizeApp",typeof(DataProtectorTokenProvider<ApplicationUser>));
           

            //Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

           //Adding Jwt Bearer
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey= true,
                   ValidAudience = Configuration["JWT:ValidAudience"],
                   ValidIssuer = Configuration["JWT:ValidIssuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:Secret"])),


               };
           })
         
      /*.AddGoogle(options =>
        {
                IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");

            options.ClientId = googleAuthNSection["767916686704-fql4bubmbka31ftnadb70t656pa5kvab.apps.googleusercontent.com"];
            options.ClientSecret = googleAuthNSection["_IASP8rZypXBJdYi3TMO8xyb"];
        })*/;



            var config = new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new MapperSetting());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        
        }

     
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<ServizeDBContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
           // app.UseSerilogRequestLogging();
           
            app.UseRouting();
           
            app.UseAuthentication();   // add to pipline 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {       
                endpoints.MapControllers();             
            });
        }
    }
}
