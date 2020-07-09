using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // authorizationflow type
            string client_id, // client id
            string redirect_uri,
            string scope, // what info I want (email, grandma, tel)
            string state // random string generated to confirm that we are going back to the same client
            )
        {
            var query = new QueryBuilder();
            query.Add("redirecturi", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }
        
        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirecturi,
            string state)
        {
            const string code = "BLABLABLABLA";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return Redirect($"{redirecturi}{query.ToString()}");
        }

        // ये वाला endpoint back channel से हिट होता है। browser को इसका पता नहीं चलता है।
        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_code,
            string refresh_token)
        {
            // some mechanism for validating the code

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
                new Claim("granny", "cookie")
            };

            var secretBytes = Encoding.ASCII.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            // वैसे Refresh Tokens के लिए अलग से logic होता है परंतु हम यहाँ पर dead simple logic को use करेंगे। 
            // हम same access token को ही भेज देंगे परंतु उसका expiration time थोड़ा increase कर देंगे।

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: refresh_token == "refresh_token" ? DateTime.Now.AddMinutes(5) : DateTime.Now.AddSeconds(5),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var response_object = new
            { 
                access_token,
                token_type = "Bearer",
                refresh_token = "thisIsJustASampleefrehToken1982"
            };

            var responseJson = JsonConvert.SerializeObject(response_object);
            var responseBytes = Encoding.ASCII.GetBytes(responseJson);
            
            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        [Authorize]
        public IActionResult Validate()
        {
            // Let's extract access token from Query string
            if(HttpContext.Request.Query.TryGetValue("access_token", out var token))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
