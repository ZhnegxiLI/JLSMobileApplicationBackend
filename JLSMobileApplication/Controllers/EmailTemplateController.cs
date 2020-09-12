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

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> Test()
        {
            /* This function show only use for degug */
            var emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/ReinitialiserPassword", new ActiverMonCompteModel()
            {
                ConfirmationLink = "test",
                Username = "test",
                Entreprise = "test",
                Phone = "test"
            });

            return View("ActiverMonCompte", new ActiverMonCompteModel()
            {
                ConfirmationLink = "test",
                Username = "test",
                Entreprise = "test",
                Phone = "test"
            });
        }
    }
}
