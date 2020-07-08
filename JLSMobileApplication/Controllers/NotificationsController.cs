using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSMobileApplication.Heplers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JLSMobileApplication.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AppSettings _appSettings;

        public NotificationsController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IActionResult EmailConfirmed(int userId, string code)
        {
            ViewData["WebsiteUrl"] = _appSettings.WebSiteUrl;
            // TODO: 之后美化页面,设置如果已经被验证过就直接跳转到登录页
            return View();
        }
    }
}