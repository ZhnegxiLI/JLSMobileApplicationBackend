using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess;
using JLSDataAccess.Repositories;
using JLSDataModel.Models.Adress;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class AdressController : Controller
    {
        private readonly AdressRepository _adress;
        private readonly JlsDbContext db;

        public AdressController(AdressRepository adressrepository, JlsDbContext context)
        {
            _adress = adressrepository;
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrUpdateShippingAdress([FromBody]Adress adress, [FromBody]int userId)
        {
            try
            {
                var adressId = await _adress.CreateOrUpdateAdress(adress);
                var userAdressId = await _adress.CreateOrUpdateUserShippingAdress(adressId,userId);
                return Json(new ApiResult()
                {
                    Data = new {
                        AdressId = adressId,
                        UserAdressId = userAdressId
                    },
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetUserFacturationAdress(int UserId)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _adress.GetUserFacturationAdress(UserId),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserShippingAdress(int UserId)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _adress.GetUserShippingAdress(UserId),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


    }
}