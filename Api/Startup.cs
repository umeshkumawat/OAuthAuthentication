using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.JWTRequirement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("DefaultAuth", null);

            services.AddAuthorization(config => 
            {
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defPolicy = defaultAuthBuilder
                .AddRequirements(new JWTRequirement.JWTRequirement())
                .Build();

                config.DefaultPolicy = defPolicy;
            });

            services.AddScoped<IAuthorizationHandler, JWTRequirementHandler>();

            services.AddHttpClient()
                .AddHttpContextAccessor();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
