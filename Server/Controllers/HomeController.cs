﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize] 
        public IActionResult Secret()
        {
            return View();
        }

        //public IActionResult Authenticate()
        //{
        //    var claims = new[] 
        //    { 
        //        new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
        //        new Claim("granny", "cookie")
        //    };

        //    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);

        //    var skey = new SymmetricSecurityKey(secretBytes);

        //    var signingCredential = new SigningCredentials(skey, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        Constants.Issuer, 
        //        Constants.Audience,
        //        claims, 
        //        DateTime.Now,
        //        DateTime.Now.AddHours(1), 
        //        signingCredential);

        //    var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

        //    return Ok(new { jwt_token = jsonToken});
        //}

        //public IActionResult Authenticate()
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
        //        new Claim("granny", "cookie")
        //    };

        //    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
        //    var key = new SymmetricSecurityKey(secretBytes);
        //    var algorithm = SecurityAlgorithms.HmacSha256;

        //    var signingCredentials = new SigningCredentials(key, algorithm);

        //    var token = new JwtSecurityToken(
        //        Constants.Issuer,
        //        Constants.Audience,
        //        claims,
        //        notBefore: DateTime.Now,
        //        expires: DateTime.Now.AddHours(1),
        //        signingCredentials);

        //    var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

        //    return Ok(new { access_token = tokenJson });
        //}
    }
}
