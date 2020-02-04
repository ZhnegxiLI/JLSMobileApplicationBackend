﻿using JLSDataAccess;
using JLSDataModel.Models.User;
using JLSMobileApplication.Heplers;
using JLSMobileApplication.Resources;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JLSMobileApplication.Auth
{
    [Route("api/[controller]/{action}/{id?}")]
    //[ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JlsDbContext db;
        private readonly IOptions<AppSettings> _appSettings;

        public AuthController(UserManager<User> userManager, JlsDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            db = dbContext;
            _appSettings = appSettings;
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel model)
        {

            if (model == null)
            {
                return Json(new ApiResult()
                {
                    Msg = "FAIL",
                    Success = false
                });
            }

            var user = await _userManager.FindByNameAsync(model.Username);

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.EmailConfirmed==false)
                {
                    return Json(new ApiResult()
                    {
                        Msg = "Your Email is not yet confirmed, please confirm your email and login again",
                        Success = false
                    });
                }
                var token = GenerateToken(user.Id);
                return Json(new ApiResult()
                {
                    Data = token,
                    Msg = "OK",
                    Success = true
                });
            }
            else
            {
                return Json(new ApiResult()
                {
                    Msg = "Your password or username is not correct please check your login information",
                    Success = false
                });
            }
        }

        // Generate the jwt toekn 
        private string GenerateToken(int userId)
        {

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JwtSecret)),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }

    }
