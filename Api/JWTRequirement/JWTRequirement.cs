using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.JWTRequirement
{
    public class JWTRequirement : IAuthorizationRequirement
    {
        
    }

    public class JWTRequirementHandler : AuthorizationHandler<JWTRequirement>
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;

        public JWTRequirementHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JWTRequirement requirement)
        {
            if(_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Split(' ')[1];
                var serverResponse = await _httpClient.GetAsync($"http://localhost:50059/OAuth/Validate?access_token={token}");

                if(serverResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
