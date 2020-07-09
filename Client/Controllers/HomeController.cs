using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var serverResponse = await AccessTokenRefreshWrapper(() => SecuredGetRequest("http://localhost:52703/Secret/Index"));

            return View();
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await _httpClient.GetAsync("url");
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
            Func<Task<HttpResponseMessage>> initialRequest
            )
        {
            var response = await initialRequest();

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
            {
                await RefreshAccessToken();
                response = await initialRequest();
            }
            
            return response;

        }

        private async Task RefreshAccessToken()
        {
            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");
            var client = _httpClientFactory.CreateClient();

            var requestData = new Dictionary<string, string> {
                { "grant_type", "refresh_token"},
                { "refresh_token", refresh_token}
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:50059/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var basicCredentials = "username:password";
            var encodedCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var base64Credentials = Convert.ToBase64String(encodedCredentials);

            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            var response = await client.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token");


            // इस point पर हमे access_token और refresh_token मिल गए है। अब हमें user को बिना बताए इस नयी access_token से Authenticate & SignIn करना है।

            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie");

            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);

            await HttpContext.SignInAsync("clientCookie", authInfo.Principal, authInfo.Properties);
        }
    }
}
