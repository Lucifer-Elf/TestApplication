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
using Servize.Authentication;
using Servize.Domain.Mapper;
using Servize.Domain.Model.OrderDetail;
using Servize.Utility.ServiceCollection;
using System;
using System.Text;

namespace Servize
{
    public class Startup
    {
        readonly string _policyName = "MyPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.CustomCors(_policyName);

            // All Scoped file are added in this function
            services.AddScopedDependency();


            string connectionString = "Server=servizetest.database.windows.net;" +
                                            "Database=serviceTestDb;" +
                                           " User Id =servizeAdmin;" +
                                            "Password=@Lfred1205;" +
                                            "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            //EnitiyFrameWork
            services.AddDbContext<ServizeDBContext>(options => options.UseSqlServer(connectionString));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // instance of http 
            services.AddScoped(sp => Cart.GetCart(sp));  // diffenrt instance to differnt user

            //Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ServizeDBContext>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddTokenProvider("ServizeApp", typeof(DataProtectorTokenProvider<ApplicationUser>));


            var tokenParameter = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = false,
                ValidateLifetime = true

            };

            //Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .GoogleService()
                .AddJwtService(tokenParameter)
;

            services.AddControllers(); // controller Registered
            services.AddSession(options =>
            {
                options.Cookie.Name = "otp";
                options.IdleTimeout = TimeSpan.FromSeconds(45);
                options.Cookie.IsEssential = true;
            });

            var config = new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new MapperSetting());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddDistributedMemoryCache();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton(tokenParameter);
            services.AddCustomSwagger();
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

            app.ApplicationBuilderPipeline(_policyName);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
