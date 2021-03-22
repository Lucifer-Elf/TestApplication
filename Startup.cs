using IdentityServer4;
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
using NSwag;
using NSwag.Generation.Processors.Security;
using Servize.Authentication;
using Servize.Controllers;
using Servize.Domain.Mapper;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Repositories;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servize
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "MyPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddCors(o => o.AddPolicy(name: MyAllowSpecificOrigins, builder =>
            {
                builder.WithOrigins("https://localhost:3000", "https://localhost:5001")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));



            services.AddScoped<VendorRespository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ClientRepository>();
            services.AddScoped<CartController>();
            services.AddScoped<Utility.Utilities>();
            services.AddScoped<Cart>();
            services.AddScoped<ContextTransaction>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<AccountRepository>();

            /* string connectionString = @$"Server={AzureVault.GetValue("DbServer")};
                                         Database={AzureVault.GetValue("DatabaseName")};
                                         User Id ={AzureVault.GetValue("DbUserId")};
                                         Password={AzureVault.GetValue("DbPassword")};
                                         MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";*/

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

           //Adding Jwt Bearer
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.TokenValidationParameters = tokenParameter;
           })

     .AddGoogle(options =>
        {
            options.ClientId = "767916686704-fql4bubmbka31ftnadb70t656pa5kvab.apps.googleusercontent.com";
            options.ClientSecret = "_IASP8rZypXBJdYi3TMO8xyb";
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            // to change call back Url
            //options.CallbackPath
        });

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
            // services.AddApplicationInsightsTelemetry();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Servize API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Alfred Paul",
                        Email = "Alfred@Servize.com",
                        Url = "https://twitter.com/spboyer"
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "under Servize.com",
                        Url = "https://example.com/license"
                    };
                };
                config.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "My Authentication",
                    Flow = OpenApiOAuth2Flow.Implicit,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                    {
                        {"api1", "My API"}

                    },
                            TokenUrl = "http://localhost:5000/connect/token",
                            AuthorizationUrl = "http://localhost:5000/connect/authorize",

                        },
                    }
                });

                config.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            });

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
            app.UseOpenApi();
            app.UseSwaggerUi3();
            // app.UseSerilogRequestLogging();
            
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);       
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
