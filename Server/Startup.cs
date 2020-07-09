using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config => 
                {
                    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                    var skey = new SymmetricSecurityKey(secretBytes);

                    config.Events = new JwtBearerEvents 
                    { 
                        OnMessageReceived = ctx => 
                        {
                            if (ctx.Request.Query.ContainsKey("access_token"))
                            {
                                ctx.Token = ctx.Request.Query["access_token"];
                            }

                            return Task.CompletedTask;
                        }
                    };
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero, // ये यहाँ पर केवल access token को ज़बरदस्ती expire करने के लिए है। इसको बाद में remove कर देंगे।
                        ValidIssuer = Constants.Issuer,
                        ValidAudience = Constants.Audience,
                        IssuerSigningKey = skey
                    };
                });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

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
