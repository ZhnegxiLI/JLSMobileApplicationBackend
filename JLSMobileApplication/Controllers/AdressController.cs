using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
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
        private readonly IAdressRepository _adress;

        public AdressController(IAdressRepository adressrepository)
        {
            _adress = adressrepository;
        }
        public class CreateOrUpdateAdressCriteria
        {
            public CreateOrUpdateAdressCriteria()
            {
                this.adress = new Adress();
            }
            public Adress adress { get; set; }
            public int userId { get; set; }
            public string type { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrUpdateAdress([FromBody]CreateOrUpdateAdressCriteria criteria)
        {
            try
            {
                var adressId = await _adress.CreateOrUpdateAdress(criteria.adress);
                long userAdressId = 0;
                if ( criteria.type == "shippingAdress")
                {
                    userAdressId = await _adress.CreateUserShippingAdress(adressId, criteria.userId);
                }
                else if (criteria.type == "facturationAdress")
                {
                    userAdressId = await _adress.CreateFacturationAdress(adressId, criteria.userId);
                }
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
        public async Task<JsonResult> GetUserDefaultShippingAdress(int UserId)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _adress.GetUserDefaultShippingAdress(UserId),
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