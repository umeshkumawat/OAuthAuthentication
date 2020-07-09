using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // हम यहा केवल AddAuthentication को use कर रहे है। क्योंकि authorization तो authorization-server के द्वारा होगा।
            services.AddAuthentication(config => 
            {
                // 1. What are we going to use for Authenticaion
                // we check the cookie to confirm that we are authenticated
                // (हर बार client पर authentication के लिए cookie को use करेंगे)
                config.DefaultAuthenticateScheme = "ClientCookie";

                // 2. What are we going to do when we signin
                // When we signin we'll deal out a cookie (जब हम signin करेंगे तब client और web-browser के बीच cookie का आदान प्रदान होगा।)
                config.DefaultSignInScheme = "ClientCookie";

                // 3. How are we going to check up, we'r allowed to do something
                //use this to check if we are allowed to do something
                config.DefaultChallengeScheme = "OurServer";
                
            })
                .AddCookie("ClientCookie") // "ClientCookie" ये एक scheme name है। हम कुछ भी scheme name दे सकते है।
                .AddOAuth("OurServer", config => // "OurServer" ये भी एक scheme name है।
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "http://localhost:50059/oauth/authorize";
                    config.TokenEndpoint = "http://localhost:50059/oauth/Token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context => 
                        {
                            var accessToken = context.AccessToken;
                            var base64payload = accessToken.Split('.')[1];

                            base64payload = base64payload.PadRight(base64payload.Length + (base64payload.Length * 3) % 4, '=');  // add padding
                            var data = Convert.FromBase64String(base64payload);


                            var bytes = Convert.FromBase64String(base64payload);
                            var jsonpayload = Encoding.UTF8.GetString(bytes);

                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonpayload);

                            foreach(var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();

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
