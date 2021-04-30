using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Servize.Controllers;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Repositories;

namespace Servize.Utility.ServiceCollection
{
    public static class CustomServices
    {
        public static AuthenticationBuilder GoogleService(this AuthenticationBuilder builder)
        {
            builder.AddGoogle(options =>
             {
                 options.ClientId = "767916686704-fql4bubmbka31ftnadb70t656pa5kvab.apps.googleusercontent.com";
                 options.ClientSecret = "_IASP8rZypXBJdYi3TMO8xyb";
                 options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                 // to change call back Url
                 //options.CallbackPath
             });
            return builder;
        }

        public static AuthenticationBuilder AddJwtService(this AuthenticationBuilder builder, TokenValidationParameters parameter)
        {
            builder.AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.TokenValidationParameters = parameter;
           });
            return builder;
        }


        public static IServiceCollection AddScopedDependency(this IServiceCollection services)
        {
            services.AddScoped<VendorRespository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ClientRepository>();
            services.AddScoped<CartController>();
            services.AddScoped<Utility.Utilities>();
            services.AddScoped<Cart>();
            services.AddScoped<ContextTransaction>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<AccountRepository>();
            services.AddScoped<OrderSummaryRepository>();
            return services;

        }

        public static IServiceCollection CustomCors(this IServiceCollection services, string policyName)
        {
            services.AddCors(o => o.AddPolicy(name: policyName, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            return services;
        }

        public static IApplicationBuilder ApplicationBuilderPipeline(this IApplicationBuilder app, string corsPolicyName)
        {

            // app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors(corsPolicyName);
            app.UseAuthentication();   // add to pipline 
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseSession();
            return app;
        }
    }
}
