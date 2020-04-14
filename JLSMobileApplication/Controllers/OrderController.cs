using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.ViewModels;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    //[Authorize] //todo add authorize
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdressRepository _adressRepository;
        private readonly IMapper _mapper;

        public OrderController(IMapper mapper, IOrderRepository order, IAdressRepository adress)
        {
            _orderRepository = order;
            _adressRepository = adress;
            _mapper = mapper;
        }
        public class SaveOrderCriteria{
            public SaveOrderCriteria()
            {
                this.References = new List<OrderProductViewModelMobile>();
            }
            public long ShippingAdressId { get; set; }
            public long FacturationAdressId { get; set; }
            public int UserId { get; set; }
            public List<OrderProductViewModelMobile> References { get; set; }

        }

        [HttpPost]
        public async Task<JsonResult> SaveOrder([FromBody] SaveOrderCriteria criteria)
        {
            try
            {
                var adress = await _adressRepository.GetAdressByIdAsync(criteria.ShippingAdressId);
                return Json(new ApiResult()
                {
                    Data = await _orderRepository.SaveOrder(criteria.References, adress, criteria.FacturationAdressId,criteria.UserId),
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
        public async Task<JsonResult> GetOrdersListByUserId(int UserId, string StatusCode, string Lang)
        {
            try
            {
               
                return Json(new ApiResult()
                {
                    Data = await _orderRepository.GetOrdersListByUserId(UserId, StatusCode, Lang),
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
        public async Task<JsonResult> GetOrdersListByOrderId(long OrderId, string Lang)
        {
            try
            {

                return Json(new ApiResult()
                {
                    Data = await _orderRepository.GetOrdersListByOrderId(OrderId, Lang),
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