using AutoMapper;
using JLSDataAccess;

using JLSDataModel.Models.User;
using JLSMobileApplication.Resources;
using JLSMobileApplication.Services;
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
                var userIdentity = _mapper.Map<User>(model);// 将UserRegistrationView 映射到User(转化为User(type:User))
                var result = await _userManager.CreateAsync(userIdentity, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return new BadRequestObjectResult(result.Errors);
                }
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(userIdentity);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userIdentity.Id, code = code }, HttpContext.Request.Scheme);

                //Email service 
                //var r = _emailService.SendEmail(userIdentity.Email, "Confirmation votre compte", callbackUrl);

                //await db.Customers.AddAsync(new Customer { IdentityId = userIdentity.Id, Location = model.Location });
                //await _appDbContext.SaveChangesAsync();

                return new OkObjectResult(callbackUrl);
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
