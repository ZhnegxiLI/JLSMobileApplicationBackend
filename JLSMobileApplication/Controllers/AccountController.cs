using AutoMapper;
using JLSDataAccess;
using JLSDataModel.Models.Adress;
using JLSDataModel.Models.User;
using JLSMobileApplication.Resources;
using JLSMobileApplication.Services;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private JlsDbContext db;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, JlsDbContext dbContext, IMapper mapper, IEmailService emailService)
        {
            _mapper = mapper;
            _userManager = userManager;
            db = dbContext;
            _emailService = emailService;
        }
        /// <summary>
        /// 注册controller
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // Step 1: 建立新地址
                var adress = _mapper.Map<Adress>(model);
                await db.AddAsync(adress);
                await db.SaveChangesAsync();

                // Step2: 整理用户信息(以邮箱作用户名)
                var userIdentity = _mapper.Map<User>(model);// 将UserRegistrationView 映射到User(转化为User(type:User))
                userIdentity.UserName = userIdentity.Email;
                userIdentity.FacturationAdressId = adress.Id;
                var result = await _userManager.CreateAsync(userIdentity, model.Password);
                
                // Step3: 检查注册是否成功 
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                   return Json(new ApiResult()
                    {
                        Msg = result.Errors.ToString(),
                        Success = false
                    });
                }
                // Step3: 加入用户权限
                var result1 = await _userManager.AddToRoleAsync(userIdentity, "Client");
                if (!result1.Succeeded)
                {
                    
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Json(new ApiResult()
                    {
                        Msg = result.Errors.ToString(),
                        Success = false
                    });
                }

                // Step5: 检查是否将发货地址与发票地址设置为同一地址 
                if (model.UseSameAddress == true)
                {
                    var userShippingAdress = new UserShippingAdress();
                    userShippingAdress.ShippingAdressId = adress.Id;
                    userShippingAdress.UserId = userIdentity.Id;

                    await db.AddAsync(userShippingAdress);
                    await db.SaveChangesAsync();
                }

                // Step6: 发送确认邮件
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userIdentity.Id, code = code }, HttpContext.Request.Scheme);
                string body = "Please confirm your email:" + callbackUrl;
                //Email service 
                var r = _emailService.SendEmail(userIdentity.Email, "Confirmation votre compte", body);

                return Json(new ApiResult()
                {
                    DataExt = body,
                    Data= r,
                    Msg = "OK",
                    Success = true
                });
            }
            catch (System.Exception exc )
            {

                throw exc;
            }
       
        }


        /// <summary>
        /// 邮箱验证生成器,并进行验证后的操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId)||string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new JsonResult("ERROR"); // cannot find the user
            }
            if (user.EmailConfirmed)
            {
                return Redirect("/login"); //  
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return RedirectToAction("EmailConfirmed", "Notifications", new { user, code });
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.ToString());
                }
                return new JsonResult(errors);
            }
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public IActionResult ResetPassword(string token)
        {
            return View();
        }


        [HttpPost("[action]")]
        public IActionResult ResetPassword(ResetPasswordViewModel  obj)
        {
            User userIdentity = _userManager.FindByNameAsync(obj.UserName).Result;

            IdentityResult result = _userManager.ResetPasswordAsync
                      (userIdentity, obj.Token, obj.Password).Result;
            if (result.Succeeded)
            {
                ViewBag.Message = "Password reset successful!";
                return View("Success");
            }
            else
            {
                ViewBag.Message = "Error while resetting the password!";
                return View("Error");
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetLink(string username)
        {
            User user = _userManager.FindByNameAsync(username).Result;

            if (user == null || !(_userManager.
                  IsEmailConfirmedAsync(user).Result))
            {
                ViewBag.Message = "Error while  resetting your password!";
                return View("Error");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword",
                "Account", new { token = token },HttpContext.Request.Scheme);

   
            return new OkObjectResult(resetLink);
        }

    }
}
