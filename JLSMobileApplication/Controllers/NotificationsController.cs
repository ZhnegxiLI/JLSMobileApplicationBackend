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
            ViewData["WebSiteUrl"] = _appSettings.WebSiteUrl;
            return View();
        }

        public IActionResult ResentEmail(int userId, string code)
        {
            ViewData["WebSiteUrl"] = _appSettings.WebSiteUrl;
            return View();
        }
    }
}