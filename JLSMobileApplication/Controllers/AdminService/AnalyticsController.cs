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
        private readonly IOrderRepository _orderRepository;
        private readonly JlsDbContext db;

        public AnalyticsController(IOrderRepository orderRepository, IAnalyticsReporsitory analyticsRepository, JlsDbContext context)
        {
            _analytics = analyticsRepository;
            this._orderRepository = orderRepository;
            db = context;
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
        /* TODO:前端加入 */
        [HttpGet]
        public async Task<JsonResult> GetRecentOrderInfo(string Lang)
        {
            try
            {
                var statusIdProgressingId = db.ReferenceItem.Where(p => p.Code == "OrderStatus_Progressing").Select(p => p.Id).FirstOrDefault();
                // last 10 days 
                var progressingList = (await _orderRepository.AdvancedOrderSearchByCriteria(Lang, null, null,null, null, statusIdProgressingId)).Skip(0).Take(10);//DateTime.Now.AddDays(-10)

                var statusIdValidedId = db.ReferenceItem.Where(p => p.Code == "OrderStatus_Valid").Select(p => p.Id).FirstOrDefault();
                // last 10 days 
                var validedList = (await _orderRepository.AdvancedOrderSearchByCriteria(Lang, null, null, DateTime.Today.Date, null, statusIdValidedId)).Skip(0).Take(10);

                return Json(new
                {
                    ProgressingOrderList = progressingList,
                    ValidedOrderList = validedList
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /* TODO:前端加入 */
        [HttpGet]
        public async Task<JsonResult> GetSalesPerformanceByYearMonth()
        {
            try
            {
                var result = await _analytics.GetSalesPerformanceByYearMonth();
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopSaleProduct(string Lang, int Limit)
        {
            try
            {
                var result = await _analytics.GetTopSaleProduct(Lang, Limit);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public JsonResult GetBestClientWidget(int Limit)
        {
            try
            {
                var result =  _analytics.GetBestClientWidget(Limit);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        

        [HttpGet]
        public async Task<JsonResult> GetInternalExternalSalesPerformance(string Lang)
        {
            try
            {
                var result = await _analytics.GetInternalExternalSalesPerformance(Lang);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesPerformanceByStatus(string Lang)
        {
            try
            {
                var result = await _analytics.GetSalesPerformanceByStatus(Lang);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTeamMemberSalesPerformance()
        {
            try
            {
                var result = await _analytics.GetTeamMemberSalesPerformance();
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}