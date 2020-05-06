using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataAccess.Repositories;
using JLSDataModel.Models.Adress;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Authorize]
    [Route("admin/[controller]/{action}/{id?}")]
    [ApiController]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsReporsitory  _analytics;

        public AnalyticsController(IAnalyticsReporsitory analyticsRepository)
        {
            _analytics = analyticsRepository;
        }

        [HttpGet]
        public async Task<JsonResult> GetAdminSalesPerformanceDashboard(string Lang)
        {
            try
            {
                var result = await _analytics.GetAdminSalesPerformanceDashboard(Lang);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}