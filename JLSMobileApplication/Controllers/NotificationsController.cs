using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult EmailConfirmed(int userId, string code)
        {
            // TODO: 之后美化页面,设置如果已经被验证过就直接跳转到登录页
            return View();
        }
    }
}