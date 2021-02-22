using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Session;
using Servize.Authentication;
using Servize.Controllers;
using Servize.Domain.Mapper;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Repositories;
using System;
using System.Text;

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
            services.AddScoped<ServizeCategoryRepository>();
            services.AddScoped<ServizeCartController>();
            services.AddScoped<Utility.Utilities>();
            services.AddScoped<Cart>();

            //EnitiyFrameWork
            services.AddDbContext<ServizeDBContext>(options => options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=TestDatabase;Trusted_Connection=True;"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // instance of http 
            services.AddScoped(sp => Cart.GetCart(sp));  // diffenrt instance to differnt user


            //Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ServizeDBContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddTokenProvider("ServizeApp", typeof(DataProtectorTokenProvider<ApplicationUser>));


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
                   ValidateIssuerSigningKey = true,
                   ValidAudience = Configuration["JWT:ValidAudience"],
                   ValidIssuer = Configuration["JWT:ValidIssuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:Secret"])),


               };
           })         

     .AddGoogle(options =>
        {
            options.ClientId = Utility.Configurations.Configuration.GetParameterValue("GoogleClientId");
            options.ClientSecret = Utility.Configurations.Configuration.GetParameterValue("GoogleSecret");

            // to change call back Url
            //options.CallbackPath
        });



            var config = new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new MapperSetting());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDistributedMemoryCache();

            services.AddSession(/*options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            }*/);

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

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
