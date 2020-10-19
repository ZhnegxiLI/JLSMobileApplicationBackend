using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSMobileApplication.Services;
using JLSMobileApplication.Services.EmailTemplateModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplateController : Controller
    {

        private IViewRenderService _view = null;

        public EmailTemplateController(IViewRenderService view)
        {
            _view = view;
        }

        public class test
        {
            public string Email { get; set; }

            public string Message { get; set; }
        }
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> Test()
        {
            /* This function show only use for degug */
      

            return View("ClientMessageToAdmin", new test
            {
                Email = "test@gmail.com",
                Message = "test"
              
            });
        }
    }
}
